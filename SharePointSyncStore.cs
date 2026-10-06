using Microsoft.SharePoint.Client;
using SPFile = Microsoft.SharePoint.Client.File;
using SPList = Microsoft.SharePoint.Client.List;

namespace CamulosSharePointUpload;

internal sealed class SharePointSyncStore : ISyncStore
{
    private readonly ClientContext context;
    private readonly SPList library;
    private readonly string libraryRoot;
    private readonly string root;

    public SharePointSyncStore(ClientContext context, string libraryName, string folder)
    {
        this.context = context;
        library = Guid.TryParse(libraryName, out var id) ? context.Web.Lists.GetById(id) : context.Web.Lists.GetByTitle(libraryName);
        context.Load(library, l => l.BaseType, l => l.RootFolder.ServerRelativePath);
        context.ExecuteQuery();
        if (library.BaseType != BaseType.DocumentLibrary) throw new ArgumentException("Select a SharePoint document library.");
        libraryRoot = library.RootFolder.ServerRelativePath.DecodedUrl.TrimEnd('/');
        if (!string.IsNullOrEmpty(folder)) SyncPaths.Validate(folder);
        if (!string.IsNullOrEmpty(folder) && SyncPaths.IsExcluded(folder))
            throw new ArgumentException("The selected folder is excluded from document sync: " + folder);
        if (IsForms(folder)) throw new ArgumentException("The library's system Forms folder cannot be synced.");
        root = libraryRoot + (string.IsNullOrEmpty(folder) ? "" : "/" + folder);
        Console.WriteLine("SharePoint scope: " + root);
    }

    private static bool IsForms(string path) => path != null && (path.Equals("Forms", StringComparison.OrdinalIgnoreCase) || path.StartsWith("Forms/", StringComparison.OrdinalIgnoreCase));
    private string Url(string relative)
    {
        SyncPaths.Validate(relative);
        if (root == libraryRoot && IsForms(relative)) throw new IOException("The library's system Forms folder is protected.");
        return root + "/" + relative;
    }
    private SPFile GetFile(string relative) => context.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(Url(relative)));
    private Folder GetFolder(string relative) => context.Web.GetFolderByServerRelativePath(ResourcePath.FromDecodedUrl(Url(relative)));

    public void ValidatePath(string path)
    {
        SyncPaths.ValidateSharePoint(path);
        if (Url(path).TrimStart('/').Length > 400)
            throw new IOException("SharePoint path exceeds 400 characters: " + path +
                ". Shorten the path or choose a shallower sync root.");
    }

    public SyncSnapshot Scan()
    {
        var scope = context.Web.GetFolderByServerRelativePath(ResourcePath.FromDecodedUrl(root));
        context.Load(scope, f => f.Exists);
        context.ExecuteQuery();
        if (!scope.Exists) throw new DirectoryNotFoundException("SharePoint scope does not exist: " + root);
        var result = new SyncSnapshot();
        var query = new CamlQuery
        {
            FolderServerRelativePath = ResourcePath.FromDecodedUrl(root),
            DatesInUtc = true,
            ViewXml = "<View Scope='RecursiveAll'><Query><OrderBy><FieldRef Name='ID'/></OrderBy></Query>" +
                "<ViewFields><FieldRef Name='FileRef'/><FieldRef Name='FSObjType'/><FieldRef Name='Modified'/>" +
                "<FieldRef Name='File_x0020_Size'/></ViewFields><RowLimit Paged='TRUE'>2000</RowLimit></View>"
        };
        do
        {
            var items = library.GetItems(query);
            context.Load(items);
            context.ExecuteQuery();
            foreach (var item in items)
            {
                string url = (string)item["FileRef"];
                if (url.Equals(root, StringComparison.OrdinalIgnoreCase)) continue;
                if (!url.StartsWith(root + "/", StringComparison.OrdinalIgnoreCase))
                    throw new IOException("SharePoint returned an item outside the sync scope: " + url);
                string relative = url[(root.Length + 1)..];
                if (root == libraryRoot && IsForms(relative)) continue;
                if (SyncPaths.IsExcluded(relative))
                {
                    result.Exclude(relative);
                    continue;
                }
                if (item.FileSystemObjectType == FileSystemObjectType.Folder) result.AddFolder(relative);
                else result.AddFile(new(relative, Utc((DateTime)item["Modified"]), Convert.ToInt64(item["File_x0020_Size"])));
            }
            query.ListItemCollectionPosition = items.ListItemCollectionPosition;
        } while (query.ListItemCollectionPosition != null);
        return result;
    }

    private static DateTime Utc(DateTime value) => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    public void CreateFolder(string path)
    {
        string parent = SyncPaths.Parent(path);
        // Scope root must already exist; never silently create a different library/folder.
        var parentFolder = context.Web.GetFolderByServerRelativePath(ResourcePath.FromDecodedUrl(parent == "" ? root : Url(parent)));
        parentFolder.Folders.AddUsingPath(ResourcePath.FromDecodedUrl(Url(path)), new FolderCollectionAddParameters { Overwrite = false });
        context.ExecuteQuery();
    }

    private SyncFile ReadFileState(string path)
    {
        // Direct ListItem reads can use the site's regional time. Use the same
        // explicit UTC CAML mode as Scan for every timestamp comparison.
        string escapedUrl = System.Security.SecurityElement.Escape(Url(path));
        var items = library.GetItems(new CamlQuery
        {
            FolderServerRelativePath = ResourcePath.FromDecodedUrl(root),
            DatesInUtc = true,
            ViewXml = "<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='FileRef'/>" +
                "<Value Type='Text'>" + escapedUrl + "</Value></Eq></Where></Query>" +
                "<ViewFields><FieldRef Name='FileRef'/><FieldRef Name='FSObjType'/>" +
                "<FieldRef Name='Modified'/><FieldRef Name='File_x0020_Size'/></ViewFields>" +
                "<RowLimit>2</RowLimit></View>"
        });
        context.Load(items);
        context.ExecuteQuery();
        if (items.Count != 1 || items[0].FileSystemObjectType != FileSystemObjectType.File)
            throw new IOException("SharePoint file is missing or ambiguous: " + path);
        var item = items[0];
        return new(path, Utc((DateTime)item["Modified"]), Convert.ToInt64(item["File_x0020_Size"]));
    }

    private SPFile Inspect(SyncFile expected)
    {
        var actual = ReadFileState(expected.Path);
        if (actual.Length != expected.Length || actual.ModifiedUtc != expected.ModifiedUtc)
            throw new IOException("SharePoint file changed during sync: " + expected.Path);
        return GetFile(expected.Path);
    }
    public void Verify(SyncFile file) => Inspect(file);
    public Stream OpenRead(SyncFile file)
    {
        var result = GetFile(file.Path).OpenBinaryStream();
        context.ExecuteQuery();
        return result.Value;
    }
    public void Write(SyncFile source, SyncFile previous, Stream content)
    {
        if (previous != null) Verify(previous);
        var folder = context.Web.GetFolderByServerRelativePath(ResourcePath.FromDecodedUrl(SyncPaths.Parent(source.Path) == "" ? root : Url(SyncPaths.Parent(source.Path))));
        var file = SharePointFileTransfer.Upload(context, folder, Url(source.Path), content, source.Length, previous != null);
        var item = file.ListItemAllFields;
        context.Load(item);
        context.ExecuteQuery();
        DateTime expectedModified = SyncPaths.Seconds(source.ModifiedUtc);
        var editorValue = item["Editor"] as FieldUserValue
            ?? throw new IOException("SharePoint did not return the uploaded file's Editor: " + source.Path);
        var editor = context.Web.SiteUsers.GetById(editorValue.LookupId);
        context.Load(editor, user => user.LoginName);
        // Form-value dates are interpreted in the site's regional time zone,
        // not in UTC or in the Linux machine's local time zone.
        var siteLocalTime = context.Web.RegionalSettings.TimeZone.UTCToLocalTime(expectedModified);
        context.ExecuteQuery();
        var results = item.ValidateUpdateListItem(
            SharePointTimestamp.FormValues(siteLocalTime.Value, editor.LoginName), true, "");
        context.ExecuteQuery();
        SharePointTimestamp.EnsureSuccess(results);

        var actual = ReadFileState(source.Path);
        if (actual.Length != source.Length || SyncPaths.Seconds(actual.ModifiedUtc) != expectedModified)
            throw new IOException("SharePoint did not preserve the uploaded size/timestamp: " + source.Path +
                $". Expected {source.Length} bytes / Modified {expectedModified:O}; " +
                $"received {actual.Length} bytes / Modified {actual.ModifiedUtc:O}.");
    }
    public void RecycleFile(SyncFile file)
    {
        Inspect(file).Recycle();
        context.ExecuteQuery();
    }
    public void RecycleEmptyFolder(string path)
    {
        var folder = GetFolder(path);
        context.Load(folder.Files);
        context.Load(folder.Folders);
        context.ExecuteQuery();
        if (folder.Files.Count != 0 || folder.Folders.Count != 0)
            throw new IOException("Remote folder is no longer empty; deletion stopped: " + path);
        folder.Recycle();
        context.ExecuteQuery();
    }
}

internal static class SharePointTimestamp
{
    public static IList<ListItemFormUpdateValue> FormValues(DateTime siteLocalTime, string editorLogin) =>
        new List<ListItemFormUpdateValue>
        {
            new()
            {
                FieldName = "Modified",
                FieldValue = siteLocalTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            },
            new()
            {
                FieldName = "Editor",
                FieldValue = System.Text.Json.JsonSerializer.Serialize(new[] { new { Key = editorLogin } })
            }
        };

    public static void EnsureSuccess(IList<ListItemFormUpdateValue> results)
    {
        var errors = results.Where(value => value.HasException).ToArray();
        if (errors.Length > 0)
            throw new IOException("SharePoint rejected timestamp metadata: " +
                string.Join("; ", errors.Select(value => value.FieldName + ": " + value.ErrorMessage)));
        if (!results.Any(value => value.FieldName == "Modified"))
            throw new IOException("SharePoint returned no validation result for Modified.");
    }
}

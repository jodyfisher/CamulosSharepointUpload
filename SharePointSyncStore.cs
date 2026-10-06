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

    private SPFile Inspect(SyncFile expected)
    {
        var file = GetFile(expected.Path);
        var item = file.ListItemAllFields;
        context.Load(file, f => f.Length);
        context.Load(item);
        context.ExecuteQuery();
        // Use the same library Modified field as Scan and timestamp planning.
        if (file.Length != expected.Length || Utc((DateTime)item["Modified"]) != expected.ModifiedUtc)
            throw new IOException("SharePoint file changed during sync: " + expected.Path);
        return file;
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
        item["Modified"] = expectedModified;
        item.UpdateOverwriteVersion();
        context.ExecuteQuery();

        // Read back in a separate request after updating. File.TimeLastModified is
        // not the field used by Scan; verify the library item's Modified instead.
        var verifiedFile = GetFile(source.Path);
        var verifiedItem = verifiedFile.ListItemAllFields;
        context.Load(verifiedFile, f => f.Length);
        context.Load(verifiedItem);
        context.ExecuteQuery();
        DateTime actualModified = Utc((DateTime)verifiedItem["Modified"]);
        if (verifiedFile.Length != source.Length || SyncPaths.Seconds(actualModified) != expectedModified)
            throw new IOException("SharePoint did not preserve the uploaded size/timestamp: " + source.Path +
                $". Expected {source.Length} bytes / Modified {expectedModified:O}; " +
                $"received {verifiedFile.Length} bytes / Modified {actualModified:O}.");
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

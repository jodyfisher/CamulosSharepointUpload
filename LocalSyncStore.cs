namespace CamulosSharePointUpload;

internal sealed class LocalSyncStore(string root, bool requireExisting) : ISyncStore
{
    private readonly string rootPath = Path.GetFullPath(root);
    private readonly Dictionary<string, string> knownPaths = new(StringComparer.OrdinalIgnoreCase);

    public SyncSnapshot Scan()
    {
        CheckLinks(rootPath);
        var result = new SyncSnapshot();
        knownPaths.Clear();
        if (!Directory.Exists(rootPath))
        {
            if (requireExisting || System.IO.File.Exists(rootPath))
                throw new DirectoryNotFoundException("Local directory does not exist: " + rootPath);
            return result;
        }
        void Visit(string directory)
        {
            foreach (var entry in new DirectoryInfo(directory).EnumerateFileSystemInfos())
            {
                if ((entry.Attributes & FileAttributes.ReparsePoint) != 0 || entry.LinkTarget != null)
                    throw new IOException("Symbolic links are not supported in the sync tree: " + entry.FullName);
                string relative = Path.GetRelativePath(rootPath, entry.FullName).Replace(Path.DirectorySeparatorChar, '/');
                if (!knownPaths.TryAdd(relative, entry.FullName))
                    throw new IOException("Duplicate or case-conflicting local path: " + relative);
                if (entry is DirectoryInfo)
                {
                    result.AddFolder(relative);
                    Visit(entry.FullName);
                }
                else
                {
                    var file = (FileInfo)entry;
                    result.AddFile(new(relative, file.LastWriteTimeUtc, file.Length));
                }
            }
        }
        Visit(rootPath);
        return result;
    }

    private string Resolve(string relative)
    {
        SyncPaths.Validate(relative);
        string path = rootPath;
        string prefix = "";
        // Scan indexes the real Linux casing; avoid rescanning large directories for every file.
        foreach (string part in relative.Split('/'))
        {
            prefix = prefix == "" ? part : prefix + "/" + part;
            path = knownPaths.TryGetValue(prefix, out string existing) ? existing : Path.Combine(path, part);
        }
        CheckLinks(path);
        return path;
    }

    private static void CheckLinks(string path)
    {
        // Include ancestors of the selected root so a symlink cannot redirect downloads outside it.
        for (string current = path; !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
        {
            FileSystemInfo info = Directory.Exists(current) ? new DirectoryInfo(current) : new FileInfo(current);
            if (info.LinkTarget != null || (info.Exists && (info.Attributes & FileAttributes.ReparsePoint) != 0))
                throw new IOException("Symbolic links are not supported in the sync path: " + current);
        }
    }

    public void CreateFolder(string path)
    {
        string resolved = Resolve(path);
        Directory.CreateDirectory(resolved);
        knownPaths[path] = resolved;
    }
    public void Verify(SyncFile file)
    {
        var info = new FileInfo(Resolve(file.Path));
        if (!info.Exists || info.Length != file.Length || info.LastWriteTimeUtc != file.ModifiedUtc)
            throw new IOException("Local file changed during sync: " + file.Path);
    }
    public Stream OpenRead(SyncFile file) => new FileStream(Resolve(file.Path), FileMode.Open, FileAccess.Read, FileShare.Read);

    public void Write(SyncFile source, SyncFile previous, Stream content)
    {
        string path = Resolve(previous?.Path ?? source.Path);
        if (previous != null) Verify(previous);
        else if (System.IO.File.Exists(path)) throw new IOException("A local file appeared during sync: " + source.Path);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        string temp = Path.Combine(Path.GetDirectoryName(path), ".camulos-" + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            using (var output = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                content.CopyTo(output);
                if (output.Length != source.Length) throw new IOException("Incomplete download: " + source.Path);
            }
            System.IO.File.SetLastWriteTimeUtc(temp, source.ModifiedUtc);
            if (previous != null) Verify(previous);
            else if (System.IO.File.Exists(path)) throw new IOException("A local file appeared during sync: " + source.Path);
            CheckLinks(path);
            System.IO.File.Move(temp, path, previous != null);
            knownPaths[source.Path] = path;
        }
        finally
        {
            if (System.IO.File.Exists(temp)) System.IO.File.Delete(temp);
        }
    }
    public void RecycleFile(SyncFile file) => throw new NotSupportedException("Local deletion is not supported.");
    public void RecycleEmptyFolder(string path) => throw new NotSupportedException("Local deletion is not supported.");
}

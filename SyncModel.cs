namespace CamulosSharePointUpload;

internal sealed record SyncFile(string Path, DateTime ModifiedUtc, long Length);

internal sealed class SyncSnapshot
{
    public Dictionary<string, SyncFile> Files { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Folders { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> ProtectedFolders { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int ExcludedEntries { get; private set; }

    public void Exclude(string path)
    {
        ExcludedEntries++;
        // Retain ancestors containing excluded content when planning --delete.
        for (string parent = SyncPaths.Parent(path); parent != ""; parent = SyncPaths.Parent(parent))
            ProtectedFolders.Add(parent);
    }

    public void AddFolder(string path)
    {
        SyncPaths.Validate(path);
        if (Files.ContainsKey(path) || !Folders.Add(path))
            throw new IOException("Duplicate or case-conflicting path: " + path);
    }

    public void AddFile(SyncFile file)
    {
        SyncPaths.Validate(file.Path);
        if (Folders.Contains(file.Path) || !Files.TryAdd(file.Path, file))
            throw new IOException("Duplicate or case-conflicting path: " + file.Path);
    }

    public bool SameAs(SyncSnapshot other) =>
        Folders.SetEquals(other.Folders) && Files.Count == other.Files.Count &&
        Files.All(f => other.Files.TryGetValue(f.Key, out var value) && value == f.Value);
}

internal static class SyncPaths
{
    public static void Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.StartsWith('/') || path.Contains('\\') ||
            path.Split('/').Any(s => s is "" or "." or ".." || s.Contains('\0')))
            throw new ArgumentException("Use a relative path without empty, '.' or '..' segments: " + path);
    }

    public static bool IsExcluded(string path) => path.Split('/').Any(part =>
        part.Equals(".git", StringComparison.OrdinalIgnoreCase) ||
        part.Equals(".DS_Store", StringComparison.OrdinalIgnoreCase) ||
        part.Equals("Thumbs.db", StringComparison.OrdinalIgnoreCase) ||
        part.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase) ||
        part.StartsWith("._", StringComparison.OrdinalIgnoreCase) ||
        part.EndsWith(":Zone.Identifier", StringComparison.OrdinalIgnoreCase) ||
        part.EndsWith(":Zone.Identifier:$DATA", StringComparison.OrdinalIgnoreCase));

    public static string ForSharePoint(string path)
    {
        // Only replace unsupported characters within each name, preserving separators.
        if (string.IsNullOrEmpty(path) || path.StartsWith('/') ||
            path.Split('/').Any(part => part is "" or "." or ".."))
            throw new ArgumentException("Use a relative path without empty, '.' or '..' segments: " + path);
        string result = string.Join("/", path.Split('/').Select(part =>
        {
            string name = new string(part.Select(c =>
                char.IsControl(c) || "\"*:<>?\\|".Contains(c) ? ' ' : c).ToArray()).Trim(' ');
            return name is "" or "." or ".." ? "_" : name;
        }));
        Validate(result);
        return result;
    }

    public static void ValidateSharePoint(string path)
    {
        Validate(path);
        foreach (string part in path.Split('/'))
        {
            string stem = part.Split('.')[0];
            bool reserved = part.Equals(".lock", StringComparison.OrdinalIgnoreCase) ||
                part.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase) ||
                part.Contains("_vti_", StringComparison.OrdinalIgnoreCase) ||
                part.StartsWith("~$", StringComparison.OrdinalIgnoreCase) ||
                new[] { "CON", "PRN", "AUX", "NUL" }.Contains(stem, StringComparer.OrdinalIgnoreCase) ||
                (stem.Length == 4 && char.IsAsciiDigit(stem[3]) &&
                    (stem.StartsWith("COM", StringComparison.OrdinalIgnoreCase) ||
                     stem.StartsWith("LPT", StringComparison.OrdinalIgnoreCase)));
            if (part.Any(c => char.IsControl(c) || "\"*:<>?|".Contains(c)) ||
                part.StartsWith(' ') || part.EndsWith(' ') || reserved)
                throw new IOException("Unsupported SharePoint name: " + path +
                    ". Rename this local entry before syncing.");
        }
    }

    public static int Depth(string path) => path.Count(c => c == '/');
    public static string Parent(string path) => path.Contains('/') ? path[..path.LastIndexOf('/')] : "";
    public static DateTime Seconds(DateTime value) => new(value.Ticks - value.Ticks % TimeSpan.TicksPerSecond, DateTimeKind.Utc);
}

internal interface ISyncStore
{
    SyncSnapshot Scan();
    void CreateFolder(string path);
    void Verify(SyncFile file);
    Stream OpenRead(SyncFile file);
    void Write(SyncFile source, SyncFile previous, Stream content);
    void RecycleFile(SyncFile file);
    void RecycleEmptyFolder(string path);
}

internal sealed record SyncTransfer(SyncFile Source, SyncFile Previous);
internal sealed record SyncPlan(string[] CreateFolders, SyncTransfer[] Transfers,
    SyncFile[] DeleteFiles, string[] DeleteFolders, int Skipped);

internal static class SyncPlanner
{
    public static SyncPlan Build(SyncSnapshot source, SyncSnapshot target, bool delete,
        bool localDateMode = false, DateTime? sinceUtc = null)
    {
        foreach (string path in source.Files.Keys)
            if (target.Folders.Contains(path)) throw new IOException("File/folder conflict: " + path);
        foreach (string path in source.Folders)
            if (target.Files.ContainsKey(path)) throw new IOException("Folder/file conflict: " + path);
        var transfers = source.Files.Values.Where(f => localDateMode
            ? !sinceUtc.HasValue || f.ModifiedUtc >= sinceUtc.Value
            : !target.Files.TryGetValue(f.Path, out var existing) ||
                SyncPaths.Seconds(f.ModifiedUtc) > SyncPaths.Seconds(existing.ModifiedUtc))
            .OrderBy(f => f.Path, StringComparer.OrdinalIgnoreCase)
            .Select(f => new SyncTransfer(f, target.Files.GetValueOrDefault(f.Path))).ToArray();
        return new SyncPlan(
            source.Folders.Where(f => !target.Folders.Contains(f)).OrderBy(SyncPaths.Depth)
                .ThenBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray(),
            transfers,
            delete ? target.Files.Values.Where(f => !source.Files.ContainsKey(f.Path)).ToArray() : [],
            delete ? target.Folders.Where(f => !source.Folders.Contains(f) && !target.ProtectedFolders.Contains(f)).OrderByDescending(SyncPaths.Depth).ToArray() : [],
            source.Files.Count - transfers.Length);
    }
}

internal static class SyncRunner
{
    public static SyncPlan Run(ISyncStore source, ISyncStore target, bool delete, bool dryRun, TextWriter output,
        Action<string, Exception> onError = null, bool localDateMode = false, DateTime? sinceUtc = null)
    {
        // Incomplete scans still stop the run before writes or deletion.
        var sourceSnapshot = source.Scan();
        var targetSnapshot = target.Scan();
        var failedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int errors = 0, copied = 0, created = 0, recycledFiles = 0, recycledFolders = 0;
        bool Blocked(string path)
        {
            for (string current = path; current != ""; current = SyncPaths.Parent(current))
                if (failedPaths.Contains(current)) return true;
            return false;
        }
        bool Attempt(string path, Action action)
        {
            if (Blocked(path))
            {
                output.WriteLine("SKIP " + path + " (this entry or its parent failed)");
                return false;
            }
            try { action(); return true; }
            catch (Exception error)
            {
                if (onError == null) throw;
                errors++;
                failedPaths.Add(path);
                onError(path, error);
                return false;
            }
        }
        var plan = SyncPlanner.Build(sourceSnapshot, targetSnapshot, delete, localDateMode, sinceUtc);
        // Validate planned uploads before writes; older files outside a local
        // cutoff do not need to be uploaded or stamped.
        if (target is SharePointSyncStore remote)
            foreach (string path in plan.Transfers.Select(t => t.Source.Path).Concat(plan.CreateFolders)
                .OrderBy(SyncPaths.Depth).ThenBy(p => p, StringComparer.OrdinalIgnoreCase))
                if (!Blocked(path)) Attempt(path, () => remote.ValidatePath(path));
        foreach (var folder in plan.CreateFolders)
        {
            if (Attempt(folder, () =>
            {
                output.WriteLine("CREATE FOLDER " + folder);
                if (!dryRun) target.CreateFolder(folder);
            })) created++;
        }
        foreach (var transfer in plan.Transfers)
        {
            if (Attempt(transfer.Source.Path, () =>
            {
                output.WriteLine("COPY " + transfer.Source.Path);
                if (!dryRun)
                {
                    source.Verify(transfer.Source);
                    using (var stream = source.OpenRead(transfer.Source))
                        target.Write(transfer.Source, transfer.Previous, stream);
                    source.Verify(transfer.Source);
                }
            })) copied++;
        }
        // No deletion after any item failure, incomplete scan, or changed source tree.
        if (delete && errors > 0)
            output.WriteLine("Remote deletion cancelled because some entries failed. See Errors.txt.");
        else
        {
            if (delete && !dryRun && !sourceSnapshot.SameAs(source.Scan()))
                throw new IOException("The source changed during sync. Remote deletion was cancelled; run again.");
            foreach (var file in plan.DeleteFiles)
            {
                if (Attempt(file.Path, () =>
                {
                    output.WriteLine("RECYCLE FILE " + file.Path);
                    if (!dryRun) target.RecycleFile(file);
                })) recycledFiles++;
            }
            // A failed file recycle must never lead to recycling its containing folder.
            if (errors == 0)
                foreach (var folder in plan.DeleteFolders)
                {
                    if (Attempt(folder, () =>
                    {
                        output.WriteLine("RECYCLE FOLDER " + folder);
                        if (!dryRun) target.RecycleEmptyFolder(folder);
                    })) recycledFolders++;
                    else break;
                }
        }
        output.WriteLine($"{(dryRun ? "Planned" : "Completed")}: {copied} files copied, {created} folders created, " +
            $"{plan.Skipped} files skipped, {recycledFiles} files/{recycledFolders} folders recycled, {errors} errors.");
        return plan;
    }
}

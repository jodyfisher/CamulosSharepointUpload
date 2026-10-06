namespace CamulosSharePointUpload;

internal sealed record SyncFile(string Path, DateTime ModifiedUtc, long Length);

internal sealed class SyncSnapshot
{
    public Dictionary<string, SyncFile> Files { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Folders { get; } = new(StringComparer.OrdinalIgnoreCase);

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
    public static SyncPlan Build(SyncSnapshot source, SyncSnapshot target, bool delete)
    {
        foreach (string path in source.Files.Keys)
            if (target.Folders.Contains(path)) throw new IOException("File/folder conflict: " + path);
        foreach (string path in source.Folders)
            if (target.Files.ContainsKey(path)) throw new IOException("Folder/file conflict: " + path);
        var transfers = source.Files.Values.Where(f =>
            !target.Files.TryGetValue(f.Path, out var existing) ||
            SyncPaths.Seconds(f.ModifiedUtc) > SyncPaths.Seconds(existing.ModifiedUtc))
            .OrderBy(f => f.Path, StringComparer.OrdinalIgnoreCase)
            .Select(f => new SyncTransfer(f, target.Files.GetValueOrDefault(f.Path))).ToArray();
        return new SyncPlan(
            source.Folders.Where(f => !target.Folders.Contains(f)).OrderBy(SyncPaths.Depth)
                .ThenBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray(),
            transfers,
            delete ? target.Files.Values.Where(f => !source.Files.ContainsKey(f.Path)).ToArray() : [],
            delete ? target.Folders.Where(f => !source.Folders.Contains(f)).OrderByDescending(SyncPaths.Depth).ToArray() : [],
            source.Files.Count - transfers.Length);
    }
}

internal static class SyncRunner
{
    public static SyncPlan Run(ISyncStore source, ISyncStore target, bool delete, bool dryRun, TextWriter output)
    {
        // Complete both scans and detect conflicts before performing any writes.
        var sourceSnapshot = source.Scan();
        var plan = SyncPlanner.Build(sourceSnapshot, target.Scan(), delete);
        foreach (var folder in plan.CreateFolders)
        {
            output.WriteLine("CREATE FOLDER " + folder);
            if (!dryRun) target.CreateFolder(folder);
        }
        foreach (var transfer in plan.Transfers)
        {
            output.WriteLine("COPY " + transfer.Source.Path);
            if (!dryRun)
            {
                source.Verify(transfer.Source);
                using (var stream = source.OpenRead(transfer.Source))
                    target.Write(transfer.Source, transfer.Previous, stream);
                source.Verify(transfer.Source);
            }
        }
        // No deletion after a failed transfer, incomplete scan, or changed source tree.
        if (delete && !dryRun && !sourceSnapshot.SameAs(source.Scan()))
            throw new IOException("The source changed during sync. Remote deletion was cancelled; run again.");
        foreach (var file in plan.DeleteFiles)
        {
            output.WriteLine("RECYCLE FILE " + file.Path);
            if (!dryRun) target.RecycleFile(file);
        }
        foreach (var folder in plan.DeleteFolders)
        {
            output.WriteLine("RECYCLE FOLDER " + folder);
            if (!dryRun) target.RecycleEmptyFolder(folder);
        }
        output.WriteLine($"{(dryRun ? "Planned" : "Completed")}: {plan.Transfers.Length} files copied, {plan.CreateFolders.Length} folders created, " +
            $"{plan.Skipped} files skipped, {plan.DeleteFiles.Length} files/{plan.DeleteFolders.Length} folders recycled.");
        return plan;
    }
}

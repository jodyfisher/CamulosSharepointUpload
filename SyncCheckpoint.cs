using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CamulosSharePointUpload;

internal sealed class SyncCheckpoint : IDisposable
{
    public string FilePath { get; }
    private readonly bool dryRun;
    private readonly FileStream runLock;
    private FileStream progressLog;
    private readonly Dictionary<string, SyncFile> completedFiles = new(StringComparer.OrdinalIgnoreCase);
    public string ProgressPath => FilePath + ".progress.jsonl";
    public int CompletedFiles => completedFiles.Count;

    public SyncCheckpoint(string path, bool dryRun = false)
    {
        FilePath = Path.GetFullPath(path);
        this.dryRun = dryRun;
        if (!dryRun)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            // One upload run per source/target scope may advance this checkpoint.
            runLock = new FileStream(FilePath + ".lock", FileMode.OpenOrCreate,
                FileAccess.ReadWrite, FileShare.None);
        }
    }

    public static string PathFor(string tenant, SyncJob job, string remoteScope)
    {
        string dataRoot = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(dataRoot))
            dataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
        string scope = string.Join("\n", tenant.ToLowerInvariant(),
            new Uri(job.Site).GetLeftPart(UriPartial.Authority).ToLowerInvariant(),
            remoteScope.ToLowerInvariant(),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(job.Local)));
        string key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(scope))).ToLowerInvariant();
        return Path.Combine(dataRoot, "CamulosSharePointUpload", "checkpoints", key + ".json");
    }

    public DateTime? Load()
    {
        if (!System.IO.File.Exists(FilePath)) return null;
        var state = JsonSerializer.Deserialize<CheckpointState>(System.IO.File.ReadAllText(FilePath));
        if (state == null || state.Version != 1 ||
            state.LastSuccessfulRunStartedUtc.Kind != DateTimeKind.Utc ||
            state.LastSuccessfulRunStartedUtc == DateTime.MinValue)
            throw new IOException("Invalid upload checkpoint: " + FilePath);
        return state.LastSuccessfulRunStartedUtc;
    }

    // The datetime cutoff is fixed for the whole run. A separate journal records
    // successful files without advancing past files that have not been attempted.
    public void BeginProgress(DateTime? sinceUtc)
    {
        completedFiles.Clear();
        progressLog?.Dispose();
        progressLog = null;
        string completeText = "";
        bool matching = false;
        if (System.IO.File.Exists(ProgressPath))
        {
            string text = System.IO.File.ReadAllText(ProgressPath);
            // A killed append may leave an incomplete final line. Only newline-
            // terminated entries were fully written; retry an unfinished entry.
            int end = text.LastIndexOf('\n');
            completeText = end < 0 ? "" : text[..(end + 1)];
            string[] lines = completeText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                var header = JsonSerializer.Deserialize<ProgressHeader>(lines[0]);
                if (header == null || header.Version != 1)
                    throw new IOException("Invalid upload progress: " + ProgressPath);
                matching = header.SinceUtc == sinceUtc;
                if (matching)
                    foreach (string line in lines.Skip(1))
                    {
                        var file = JsonSerializer.Deserialize<SyncFile>(line);
                        if (file == null || file.ModifiedUtc.Kind != DateTimeKind.Utc || file.Length < 0)
                            throw new IOException("Invalid upload progress entry: " + ProgressPath);
                        SyncPaths.Validate(file.Path);
                        completedFiles[file.Path] = file;
                    }
            }
        }
        if (dryRun) return;
        if (!matching)
        {
            string header = JsonSerializer.Serialize(new ProgressHeader(1, sinceUtc)) + "\n";
            string temp = ProgressPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                System.IO.File.WriteAllText(temp, header, new UTF8Encoding(false));
                System.IO.File.Move(temp, ProgressPath, true);
            }
            finally
            {
                if (System.IO.File.Exists(temp)) System.IO.File.Delete(temp);
            }
            completeText = header;
        }
        progressLog = new FileStream(ProgressPath, FileMode.Open, FileAccess.Write, FileShare.Read);
        progressLog.SetLength(Encoding.UTF8.GetByteCount(completeText));
        progressLog.Position = progressLog.Length;
    }

    public bool CanResume(SyncFile source, SyncFile remote) =>
        remote != null && remote.Length == source.Length &&
        completedFiles.TryGetValue(source.Path, out var recorded) &&
        recorded.ModifiedUtc == source.ModifiedUtc && recorded.Length == source.Length;

    public void RecordUploaded(SyncFile file)
    {
        if (dryRun) return;
        if (progressLog == null) throw new InvalidOperationException("Upload progress was not initialized.");
        byte[] line = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(file) + "\n");
        progressLog.Write(line);
        progressLog.Flush(true);
        completedFiles[file.Path] = file;
    }

    public void Complete(DateTime runStartedUtc, bool successful)
    {
        if (dryRun || !successful) return;
        if (runStartedUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Checkpoint time must be UTC.");
        string temp = FilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            System.IO.File.WriteAllText(temp, JsonSerializer.Serialize(new CheckpointState(1, SyncPaths.Seconds(runStartedUtc))));
            System.IO.File.Move(temp, FilePath, true);
            progressLog?.Dispose();
            progressLog = null;
            System.IO.File.Delete(ProgressPath);
            completedFiles.Clear();
        }
        finally
        {
            if (System.IO.File.Exists(temp)) System.IO.File.Delete(temp);
        }
    }

    public void Dispose()
    {
        try { progressLog?.Dispose(); }
        finally { runLock?.Dispose(); }
    }

    private sealed record ProgressHeader(int Version, DateTime? SinceUtc);

    private sealed record CheckpointState(int Version, DateTime LastSuccessfulRunStartedUtc);
}

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CamulosSharePointUpload;

internal sealed class SyncCheckpoint : IDisposable
{
    public string FilePath { get; }
    private readonly bool dryRun;
    private readonly FileStream runLock;

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
        }
        finally
        {
            if (System.IO.File.Exists(temp)) System.IO.File.Delete(temp);
        }
    }

    public void Dispose() => runLock?.Dispose();

    private sealed record CheckpointState(int Version, DateTime LastSuccessfulRunStartedUtc);
}

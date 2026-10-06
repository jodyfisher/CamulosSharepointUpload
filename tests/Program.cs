using CamulosSharePointUpload;
using System.Text;

var utc = new DateTime(2026, 10, 6, 0, 0, 0, DateTimeKind.Utc);
int passed = 0;
void Check(bool condition, string message = "Assertion failed") { if (!condition) throw new Exception(message); }
void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception("Expected " + typeof(T).Name);
}
void Test(string name, Action test)
{
    test(); passed++; Console.WriteLine("PASS: " + name);
}
SyncFile File(string path, int seconds = 0, long length = 1) => new(path, utc.AddSeconds(seconds), length);
SyncSnapshot Snapshot(params SyncFile[] files)
{
    var result = new SyncSnapshot();
    foreach (var file in files) result.AddFile(file);
    return result;
}
string ValidArgs = "--site https://example.sharepoint.com --library Docs --local /srv/docs";
string[] Args(string extra) => (ValidArgs + " " + extra).Split(' ', StringSplitOptions.RemoveEmptyEntries);

Test("only missing/source-newer files are transferred", () =>
{
    var plan = SyncPlanner.Build(Snapshot(File("missing"), File("newer", 2), File("equal"), File("older", -2)),
        Snapshot(File("newer"), File("equal"), File("older")), false);
    Check(plan.Transfers.Select(t => t.Source.Path).Order().SequenceEqual(new[] { "missing", "newer" }));
    Check(plan.Skipped == 2);
});
Test("subsecond timestamp rounding does not repeat an upload", () =>
{
    Check(SyncPlanner.Build(Snapshot(new SyncFile("same", utc.AddMilliseconds(900), 1)), Snapshot(File("same")), false).Transfers.Length == 0);
});
Test("download planning uses the reverse timestamp direction", () =>
{
    Check(SyncPlanner.Build(Snapshot(File("remote", 5)), Snapshot(File("remote")), false).Transfers.Length == 1);
    Check(SyncPlanner.Build(Snapshot(File("local-newer")), Snapshot(File("local-newer", 5)), false).Transfers.Length == 0);
});
Test("remote extras are retained unless delete is supplied", () =>
{
    Check(SyncPlanner.Build(Snapshot(File("keep")), Snapshot(File("keep"), File("extra")), false).DeleteFiles.Length == 0);
    Check(SyncPlanner.Build(Snapshot(File("keep")), Snapshot(File("keep"), File("extra")), true).DeleteFiles.Single().Path == "extra");
});
Test("folder create/delete order and empty folders", () =>
{
    var source = new SyncSnapshot(); source.AddFolder("a"); source.AddFolder("a/empty");
    var target = new SyncSnapshot(); target.AddFolder("old"); target.AddFolder("old/child");
    var plan = SyncPlanner.Build(source, target, true);
    Check(plan.CreateFolders.SequenceEqual(new[] { "a", "a/empty" }));
    Check(plan.DeleteFolders.SequenceEqual(new[] { "old/child", "old" }));
});
Test("file/folder type conflicts stop before any writes", () =>
{
    var source = new FakeStore(Snapshot(File("folder")));
    var targetSnapshot = new SyncSnapshot(); targetSnapshot.AddFolder("folder");
    var target = new FakeStore(targetSnapshot);
    Throws<IOException>(() => SyncRunner.Run(source, target, true, false, TextWriter.Null));
    Check(target.Events.Count == 0);
});
Test("dry run performs no writes or deletes", () =>
{
    var sourceSnapshot = Snapshot(File("new")); sourceSnapshot.AddFolder("empty");
    var source = new FakeStore(sourceSnapshot);
    var target = new FakeStore(Snapshot(File("extra")));
    SyncRunner.Run(source, target, true, true, TextWriter.Null);
    Check(target.Events.Count == 0 && source.Reads == 0);
});
Test("failed uploads prevent every remote deletion", () =>
{
    var target = new FakeStore(Snapshot(File("extra"))) { FailWrite = true };
    Throws<IOException>(() => SyncRunner.Run(new FakeStore(Snapshot(File("new"))), target, true, false, TextWriter.Null));
    Check(!target.Events.Any(e => e.StartsWith("delete")));
});
Test("logged file failure continues with the next file and cancels deletion", () =>
{
    var source = new FakeStore(Snapshot(File("a-fail"), File("b-good")));
    var target = new FakeStore(Snapshot(File("extra"))) { FailWritePath = "a-fail" };
    var failures = new List<string>();
    using var output = new StringWriter();
    SyncRunner.Run(source, target, true, false, output, (path, error) => failures.Add(path));
    Check(failures.SequenceEqual(new[] { "a-fail" }));
    Check(target.Events.SequenceEqual(new[] { "write:b-good" }));
    Check(output.ToString().Contains("deletion cancelled"));
    Check(output.ToString().Contains("1 files copied"));
});
Test("logged folder failure skips descendants and continues elsewhere", () =>
{
    var sourceSnapshot = Snapshot(File("a/file"), File("b/file"));
    sourceSnapshot.AddFolder("a"); sourceSnapshot.AddFolder("a/child"); sourceSnapshot.AddFolder("b");
    var target = new FakeStore(new()) { FailFolderPath = "a" };
    var failures = new List<string>();
    SyncRunner.Run(new FakeStore(sourceSnapshot), target, false, false, TextWriter.Null,
        (path, error) => failures.Add(path));
    Check(failures.SequenceEqual(new[] { "a" }));
    Check(target.Events.SequenceEqual(new[] { "folder:b", "write:b/file" }));
});
Test("incomplete scan prevents writes and deletes", () =>
{
    var target = new FakeStore(Snapshot(File("extra")));
    Throws<IOException>(() => SyncRunner.Run(new FakeStore(new()) { FailScan = true }, target, true, false, TextWriter.Null));
    Check(target.Events.Count == 0);
});
Test("source changes during sync cancel deletion", () =>
{
    var source = new FakeStore(Snapshot(File("new"))) { SecondSnapshot = Snapshot(File("new"), File("added")) };
    var target = new FakeStore(Snapshot(File("extra")));
    Throws<IOException>(() => SyncRunner.Run(source, target, true, false, TextWriter.Null));
    Check(!target.Events.Any(e => e.StartsWith("delete")));
});
Test("successful copies precede deletion", () =>
{
    var target = new FakeStore(Snapshot(File("extra")));
    SyncRunner.Run(new FakeStore(Snapshot(File("new"))), target, true, false, TextWriter.Null);
    Check(target.Events.SequenceEqual(new[] { "write:new", "delete:extra" }));
});
Test("reverse delete and unsupported options are rejected", () =>
{
    Throws<ArgumentException>(() => SyncOptions.Parse(Args("--download --delete")));
    Throws<ArgumentException>(() => SyncOptions.Parse(Args("--csvfile x.csv")));
    Throws<ArgumentException>(() => SyncOptions.Parse(Args("--custom 1")));
    Throws<ArgumentException>(() => SyncOptions.Parse(Args("--folder ../outside")));
    Throws<ArgumentException>(() => SyncOptions.Parse(Args("--clientid")));
});
Test("CLI aliases, dry-run and authentication check", () =>
{
    var options = SyncOptions.Parse(new[] { "-site", "https://example.sharepoint.com", "-list", "Docs", "-source", "/srv/docs", "--reverse", "--dry-run" });
    Check(options.Download && options.DryRun && !options.Delete);
    Check(SyncOptions.Parse(new[] { "--site", "https://example.sharepoint.com", "--authcheck" }).AuthCheck);
    Check(SyncOptions.Parse(new[] { "--help", "--delete" }).Help);
    Check(SyncOptions.Parse([]).Help);
});
Test("relative paths and case collisions are rejected", () =>
{
    foreach (string path in new[] { "../a", "/a", "a/../b", "a\\b", "a//b" }) Throws<ArgumentException>(() => SyncPaths.Validate(path));
    var snapshot = Snapshot(File("Report.txt"));
    Throws<IOException>(() => snapshot.AddFile(File("report.txt")));
});
Test("excluded metadata paths do not include similarly named documents", () =>
{
    foreach (string path in new[] { ".git/config", "repo/.GIT/logs/HEAD", "report.pdf:Zone.Identifier", "report.pdf:Zone.Identifier:$DATA", ".DS_Store", "Archive/._.DS_Store", "Archive/._report.pdf", "photos/Thumbs.db", "desktop.ini" })
        Check(SyncPaths.IsExcluded(path));
    foreach (string path in new[] { ".gitignore", "repo/.github/workflows/build.yml", "Zone.Identifier", "report.pdf:Zone.Identifier.txt" })
        Check(!SyncPaths.IsExcluded(path));
});
Test("excluded remote content protects ancestors from delete", () =>
{
    var target = Snapshot(File("old/extra"));
    target.AddFolder("old"); target.AddFolder("old/empty");
    target.Exclude("old/.git/config");
    var plan = SyncPlanner.Build(new(), target, true);
    Check(plan.DeleteFiles.Single().Path == "old/extra");
    Check(plan.DeleteFolders.SequenceEqual(new[] { "old/empty" }));
});
Test("SharePoint preflight rejects invalid names but accepts hash percent and spaces", () =>
{
    foreach (string path in new[] { "a:b.pdf", "a?b", "a*b", "a\"b", "a|b", "a<b", "a>b", " leading/file", "trailing ", "CON.txt", "LPT9", "_vti_config", "~$report.docx", "desktop.ini" })
        Throws<IOException>(() => SyncPaths.ValidateSharePoint(path));
    foreach (string path in new[] { "Accounts/2025 report # 100%.pdf", ".gitignore", "console.txt", "computer.txt" })
        SyncPaths.ValidateSharePoint(path);
});
Test("SharePoint name mapping only replaces problem characters", () =>
{
    Check(SyncPaths.ForSharePoint("Accounts/A&B, #100%.pdf") == "Accounts/A&B, #100%.pdf");
    Check(SyncPaths.ForSharePoint(" Folder /Invoice:2025?.pdf") == "Folder/Invoice 2025 .pdf");
    Check(SyncPaths.ForSharePoint("a\\b.pdf") == "a b.pdf");
    Check(SyncPaths.ForSharePoint("???") == "_");
    foreach (string path in new[] { "../outside", "/rooted", "a//b" })
        Throws<ArgumentException>(() => SyncPaths.ForSharePoint(path));
});
Test("post-upload timestamp form keeps UTC time and existing editor", () =>
{
    var values = SharePointTimestamp.FormValues(new DateTime(2026, 9, 29, 4, 8, 46, DateTimeKind.Utc),
        "i:0#.f|membership|user@example.com");
    Check(values.Single(v => v.FieldName == "Modified").FieldValue == "2026-09-29 04:08:46");
    using var editor = System.Text.Json.JsonDocument.Parse(values.Single(v => v.FieldName == "Editor").FieldValue);
    Check(editor.RootElement[0].GetProperty("Key").GetString() == "i:0#.f|membership|user@example.com");
});
Test("metadata validation failures are reported instead of accepting an upload timestamp", () =>
{
    var accepted = SharePointTimestamp.FormValues(utc, "user@example.com");
    SharePointTimestamp.EnsureSuccess(accepted);
    Throws<IOException>(() => SharePointTimestamp.EnsureSuccess(
        new[] { new Microsoft.SharePoint.Client.ListItemFormUpdateValue
        { FieldName = "Modified", HasException = true, ErrorMessage = "Field is read only" } }));
    Throws<IOException>(() => SharePointTimestamp.EnsureSuccess(
        Array.Empty<Microsoft.SharePoint.Client.ListItemFormUpdateValue>()));
});
string temp = Path.Combine(Path.GetTempPath(), "camulos-sync-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temp);
try
{
    Test("local scan prunes Git metadata while retaining documents", () =>
    {
        string root = Path.Combine(temp, "metadata");
        string git = Path.Combine(root, "repo", ".git", "logs"); Directory.CreateDirectory(git);
        System.IO.File.WriteAllText(Path.Combine(git, "HEAD"), "history");
        System.IO.File.WriteAllText(Path.Combine(root, "repo", ".gitignore"), "*.tmp");
        System.IO.File.WriteAllText(Path.Combine(root, "report.pdf"), "document");
        if (OperatingSystem.IsLinux())
        {
            System.IO.File.WriteAllText(Path.Combine(root, "report.pdf:Zone.Identifier"), "metadata");
            System.IO.File.WriteAllText(Path.Combine(root, "report.pdf:Zone.Identifier:$DATA"), "metadata");
            Directory.CreateSymbolicLink(Path.Combine(git, "ignored-link"), temp);
        }
        var snapshot = new LocalSyncStore(root, true).Scan();
        Check(snapshot.Files.Keys.Order().SequenceEqual(new[] { "repo/.gitignore", "report.pdf" }));
        Check(snapshot.Folders.SetEquals(new[] { "repo" }));
        Check(snapshot.ExcludedEntries == (OperatingSystem.IsLinux() ? 3 : 1));
        Check(System.IO.File.Exists(Path.Combine(git, "HEAD")));
        System.IO.File.AppendAllText(Path.Combine(git, "HEAD"), "new history");
        Check(snapshot.SameAs(new LocalSyncStore(root, true).Scan()));
    });
    Test("operating system metadata is excluded without deleting local files", () =>
    {
        string root = Path.Combine(temp, "mac-metadata"); Directory.CreateDirectory(root);
        foreach (string name in new[] { ".DS_Store", "._.DS_Store", "._report.pdf", "Thumbs.db", "desktop.ini", "report.pdf" })
            System.IO.File.WriteAllText(Path.Combine(root, name), "data");
        var snapshot = new LocalSyncStore(root, true).Scan();
        Check(snapshot.Files.Keys.SequenceEqual(new[] { "report.pdf" }));
        Check(snapshot.ExcludedEntries == 5 && Directory.GetFiles(root).Length == 6);
    });
    if (OperatingSystem.IsLinux())
    {
        Test("mapped names preserve original local paths for upload and download", () =>
        {
            string root = Path.Combine(temp, "mapped");
            string folder = Path.Combine(root, "Invoices:2025"); Directory.CreateDirectory(folder);
            string physical = Path.Combine(folder, "A&B:invoice?.pdf");
            System.IO.File.WriteAllText(physical, "old"); System.IO.File.SetLastWriteTimeUtc(physical, utc);
            var local = new LocalSyncStore(root, true);
            var snapshot = local.Scan();
            string mapped = "Invoices 2025/A&B invoice .pdf";
            var prior = snapshot.Files[mapped];
            local.Verify(prior);
            using (var read = new StreamReader(local.OpenRead(prior))) Check(read.ReadToEnd() == "old");
            var remote = Snapshot(File(mapped, 5, 3)); remote.AddFolder("Invoices 2025");
            Check(SyncPlanner.Build(snapshot, remote, true).DeleteFiles.Length == 0);
            Check(SyncPlanner.Build(snapshot, remote, true).Transfers.Length == 0);
            Check(SyncPlanner.Build(remote, snapshot, false).Transfers.Single().Source.Path == mapped);
            using var content = new MemoryStream(Encoding.UTF8.GetBytes("new"));
            local.Write(File(mapped, 5, 3), prior, content);
            Check(System.IO.File.ReadAllText(physical) == "new");
            Check(System.IO.File.GetLastWriteTimeUtc(physical) == utc.AddSeconds(5));
            Check(!Directory.Exists(Path.Combine(root, "Invoices 2025")));
        });
        Test("name replacement collisions fail before transfers", () =>
        {
            string root = Path.Combine(temp, "mapped-collision"); Directory.CreateDirectory(root);
            System.IO.File.WriteAllText(Path.Combine(root, "a:b"), "one");
            System.IO.File.WriteAllText(Path.Combine(root, "a b"), "two");
            var target = new FakeStore(new());
            Throws<IOException>(() => SyncRunner.Run(new LocalSyncStore(root, true), target, true, false, TextWriter.Null));
            Check(target.Events.Count == 0);
        });
    }
    Test("Linux scan preserves timestamps and empty directories", () =>
    {
        string root = Path.Combine(temp, "source"); Directory.CreateDirectory(Path.Combine(root, "empty"));
        System.IO.File.WriteAllText(Path.Combine(root, "a # 100%.txt"), "data");
        System.IO.File.SetLastWriteTimeUtc(Path.Combine(root, "a # 100%.txt"), utc);
        var snapshot = new LocalSyncStore(root, true).Scan();
        Check(snapshot.Folders.Contains("empty") && snapshot.Files["a # 100%.txt"].ModifiedUtc == utc);
    });
    Test("downloads replace files atomically and preserve remote time", () =>
    {
        string root = Path.Combine(temp, "download"); Directory.CreateDirectory(root);
        string path = Path.Combine(root, "file.txt"); System.IO.File.WriteAllText(path, "old");
        System.IO.File.SetLastWriteTimeUtc(path, utc);
        var target = new LocalSyncStore(root, false); var prior = target.Scan().Files["file.txt"];
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("new-data"));
        target.Write(File("file.txt", 5, 8), prior, stream);
        Check(System.IO.File.ReadAllText(path) == "new-data" && System.IO.File.GetLastWriteTimeUtc(path) == utc.AddSeconds(5));
    });
    Test("truncated downloads leave the old file intact", () =>
    {
        string root = Path.Combine(temp, "truncated"); Directory.CreateDirectory(root);
        string path = Path.Combine(root, "file.txt"); System.IO.File.WriteAllText(path, "old");
        var target = new LocalSyncStore(root, false); var prior = target.Scan().Files["file.txt"];
        using var stream = new MemoryStream([1]);
        Throws<IOException>(() => target.Write(File("file.txt", 5, 100), prior, stream));
        Check(System.IO.File.ReadAllText(path) == "old");
        Check(Directory.GetFiles(root).Length == 1);
    });
    Test("existing Linux directory casing is reused on download", () =>
    {
        string root = Path.Combine(temp, "casing"); Directory.CreateDirectory(Path.Combine(root, "Folder"));
        var target = new LocalSyncStore(root, false); target.Scan();
        using var stream = new MemoryStream([1]); target.Write(File("folder/new.txt"), null, stream);
        Check(System.IO.File.Exists(Path.Combine(root, "Folder", "new.txt")));
        Check(Directory.GetDirectories(root).Length == 1);
    });
    Test("missing upload roots cannot become empty deletion sources", () =>
    {
        Throws<DirectoryNotFoundException>(() => new LocalSyncStore(Path.Combine(temp, "missing"), true).Scan());
    });
    Test("local-to-local engine preserves data/time and the second run skips copies", () =>
    {
        string sourceRoot = Path.Combine(temp, "roundtrip-source");
        string targetRoot = Path.Combine(temp, "roundtrip-target");
        Directory.CreateDirectory(Path.Combine(sourceRoot, "nested", "empty"));
        System.IO.File.WriteAllText(Path.Combine(sourceRoot, "nested", "doc.txt"), "hello");
        System.IO.File.SetLastWriteTimeUtc(Path.Combine(sourceRoot, "nested", "doc.txt"), utc);
        var first = SyncRunner.Run(new LocalSyncStore(sourceRoot, true), new LocalSyncStore(targetRoot, false), false, false, TextWriter.Null);
        Check(first.Transfers.Length == 1);
        Check(Directory.Exists(Path.Combine(targetRoot, "nested", "empty")));
        Check(System.IO.File.ReadAllText(Path.Combine(targetRoot, "nested", "doc.txt")) == "hello");
        var second = SyncRunner.Run(new LocalSyncStore(sourceRoot, true), new LocalSyncStore(targetRoot, false), false, false, TextWriter.Null);
        Check(second.Transfers.Length == 0 && second.Skipped == 1);
    });
    Test("changed local destination is not overwritten", () =>
    {
        string root = Path.Combine(temp, "changed-target"); Directory.CreateDirectory(root);
        string path = Path.Combine(root, "file.txt"); System.IO.File.WriteAllText(path, "old");
        var target = new LocalSyncStore(root, false); var prior = target.Scan().Files["file.txt"];
        System.IO.File.WriteAllText(path, "user-edit");
        using var stream = new MemoryStream([1]);
        Throws<IOException>(() => target.Write(File("file.txt"), prior, stream));
        Check(System.IO.File.ReadAllText(path) == "user-edit");
    });
    Test("existing XML document jobs are mapped, unsafe partial jobs are rejected", () =>
    {
        string config = Path.Combine(temp, "jobs.xml");
        var database = new MigrationsDatabase();
        database.Migrations.Add(new Migration { SharepointSite = "https://example.sharepoint.com", DocLibraryName = "Docs", DocSource = "/srv/docs", StartFolder = "Projects", Username = "user@example.com" });
        database.Save(config);
        var options = SyncOptions.Parse(new[] { "--configfile", config, "--delete" });
        var job = options.Jobs().Single(); Check(job.Folder == "Projects" && job.Library == "Docs" && options.Delete);
        database.Migrations[0].ExcludeFiles = "exclusions.txt"; database.Save(config);
        Throws<ArgumentException>(() => options.Jobs());
        database.Migrations[0].ExcludeFiles = "";
        database.MetadataMigrations.Add(new MetadataMigration()); database.Save(config);
        Throws<ArgumentException>(() => options.Jobs());
    });
    if (OperatingSystem.IsLinux())
    {
        Test("symlinks in the tree and root ancestors are rejected", () =>
        {
            string root = Path.Combine(temp, "links"); Directory.CreateDirectory(root);
            Directory.CreateSymbolicLink(Path.Combine(root, "link"), temp);
            Throws<IOException>(() => new LocalSyncStore(root, true).Scan());
            string alias = Path.Combine(temp, "alias"); Directory.CreateSymbolicLink(alias, root);
            Throws<IOException>(() => new LocalSyncStore(Path.Combine(alias, "child"), false).Scan());
        });
        Test("Linux case-conflicting files are rejected", () =>
        {
            string root = Path.Combine(temp, "duplicates"); Directory.CreateDirectory(root);
            System.IO.File.WriteAllText(Path.Combine(root, "File"), "1"); System.IO.File.WriteAllText(Path.Combine(root, "file"), "2");
            Throws<IOException>(() => new LocalSyncStore(root, true).Scan());
        });
    }
}
finally { Directory.Delete(temp, true); }
Console.WriteLine($"{passed} tests passed.");

internal sealed class FakeStore(SyncSnapshot snapshot) : ISyncStore
{
    public List<string> Events { get; } = [];
    public bool FailWrite { get; init; }
    public string FailWritePath { get; init; }
    public string FailFolderPath { get; init; }
    public bool FailScan { get; init; }
    public SyncSnapshot SecondSnapshot { get; init; }
    public int Reads { get; private set; }
    private int scans;
    public SyncSnapshot Scan()
    {
        if (FailScan) throw new IOException("scan failed");
        return scans++ > 0 && SecondSnapshot != null ? SecondSnapshot : snapshot;
    }
    public void CreateFolder(string path)
    {
        if (path == FailFolderPath) throw new IOException("folder creation failed");
        Events.Add("folder:" + path);
    }
    public void Verify(SyncFile file) { }
    public Stream OpenRead(SyncFile file) { Reads++; return new MemoryStream([1]); }
    public void Write(SyncFile source, SyncFile previous, Stream content)
    {
        if (FailWrite || source.Path == FailWritePath) throw new IOException("upload failed");
        Events.Add("write:" + source.Path);
    }
    public void RecycleFile(SyncFile file) => Events.Add("delete:" + file.Path);
    public void RecycleEmptyFolder(string path) => Events.Add("delete-folder:" + path);
}

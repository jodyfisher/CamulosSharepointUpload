using System.Xml.Serialization;
using Microsoft.SharePoint.Client;

namespace CamulosSharePointUpload;

internal static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var options = SyncOptions.Parse(args);
            if (options.Help) { Console.WriteLine(SyncOptions.Usage); return 0; }
            Configuration.tenantId = options.Tenant;
            Configuration.clientId = options.ClientId;
            Configuration.o365UserName = options.User;
            SharePointAuthentication.ValidateSettings();
            if (options.AuthCheck)
            {
                using var context = Configuration.GetUserContext(options.Site);
                context.Load(context.Web, web => web.Title, web => web.Url);
                context.ExecuteQuery();
                Console.WriteLine($"Connected to: {context.Web.Title} ({context.Web.Url})");
                return 0;
            }
            bool hadErrors = false;
            foreach (var job in options.Jobs())
            {
                DateTime runStartedUtc = DateTime.UtcNow;
                bool jobErrors = false;
                Configuration.o365UserName = job.User;
                var local = new LocalSyncStore(job.Local, !options.Download);
                // A missing/unreadable/case-conflicting source fails before connecting or writing.
                var localSnapshot = local.Scan();
                if (localSnapshot.ExcludedEntries > 0)
                    Console.WriteLine($"Excluded {localSnapshot.ExcludedEntries} local metadata entries (.git, Zone.Identifier, .DS_Store, AppleDouble, Thumbs.db and desktop.ini).");
                using var context = Configuration.GetUserContext(job.Site);
                context.RequestTimeout = 180000;
                var remote = new SharePointSyncStore(context, job.Library, job.Folder, !options.LocalDateMode);
                using var checkpoint = options.SinceLastRun
                    ? new SyncCheckpoint(SyncCheckpoint.PathFor(options.Tenant, job, remote.ScopePath), options.DryRun)
                    : null;
                DateTime? sinceUtc = options.SinceUtc ?? checkpoint?.Load();
                if (checkpoint != null && sinceUtc.HasValue && sinceUtc.Value > runStartedUtc)
                    throw new IOException("The upload checkpoint/cutoff is later than this run's start. Check the system clock and --since date.");
                if (options.LocalDateMode)
                {
                    Console.WriteLine(sinceUtc.HasValue
                        ? $"Local cutoff: {sinceUtc.Value:O} (inclusive)."
                        : "No checkpoint: initial upload selects all local files.");
                    Console.WriteLine("Selected local files overwrite matching SharePoint content; SharePoint Modified dates are not preserved.");
                    if (checkpoint != null) Console.WriteLine("Checkpoint: " + checkpoint.FilePath);
                }
                Console.WriteLine($"{(options.DryRun ? "DRY RUN " : "")}{(options.Download ? "DOWNLOAD" : "UPLOAD")}: {Path.GetFullPath(job.Local)}");
                if (options.Delete) Console.WriteLine("Remote extras will be moved to the SharePoint recycle bin after successful uploads.");
                SyncRunner.Run(options.Download ? remote : local, options.Download ? local : remote,
                    options.Delete, options.DryRun, Console.Out, (path, error) =>
                    {
                        hadErrors = true;
                        jobErrors = true;
                        Console.Error.WriteLine("ERROR " + path + ": " + error.Message);
                        LogError(path, error);
                    }, options.LocalDateMode, sinceUtc);
                checkpoint?.Complete(runStartedUtc, !jobErrors);
                if (checkpoint != null && !options.DryRun)
                    Console.WriteLine(jobErrors ? "Checkpoint retained because some entries failed." : "Upload checkpoint saved.");
            }
            return hadErrors ? 1 : 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Sync stopped: " + ex.Message);
            LogError("sync", ex);
            return 1;
        }
    }

    private static void LogError(string path, Exception error)
    {
        try
        {
            System.IO.File.AppendAllText("Errors.txt",
                $"{DateTime.UtcNow:O} {path}: {error.Message}{Environment.NewLine}");
        }
        catch (Exception logError)
        {
            Console.Error.WriteLine("Could not write Errors.txt: " + logError.Message);
        }
    }
}

internal sealed record SyncJob(string Site, string Library, string Local, string Folder, string User);

internal sealed class SyncOptions
{
    public string Tenant { get; private set; } = Environment.GetEnvironmentVariable("CAMULOS_TENANT_ID") ?? "";
    public string ClientId { get; private set; } = Environment.GetEnvironmentVariable("CAMULOS_CLIENT_ID") ?? "";
    public string User { get; private set; } = "";
    public string Site { get; private set; } = "";
    public string Library { get; private set; } = "";
    public string Local { get; private set; } = "";
    public string Folder { get; private set; } = "";
    public string ConfigFile { get; private set; } = "";
    public bool SinceLastRun { get; private set; }
    public DateTime? SinceUtc { get; private set; }
    public bool LocalDateMode => SinceLastRun || SinceUtc.HasValue;
    public bool Download { get; private set; }
    public bool Delete { get; private set; }
    public bool DryRun { get; private set; }
    public bool AuthCheck { get; private set; }
    public bool Help { get; private set; }

    public static SyncOptions Parse(string[] args)
    {
        var options = new SyncOptions();
        if (args.Length == 0) { options.Help = true; return options; }
        // Help must not initiate authentication, regardless of other arguments.
        if (args.Any(a => a is "--help" or "-help" or "/help" or "-?" or "/?" or "help" or "?"))
        { options.Help = true; return options; }
        for (int i = 0; i < args.Length; i++)
        {
            string key = args[i].TrimStart('-', '/').ToLowerInvariant();
            string Value()
            {
                if (++i >= args.Length) throw new ArgumentException("Missing value for " + args[i - 1]);
                return args[i];
            }
            switch (key)
            {
                case "tenant": options.Tenant = Value().Trim(); break;
                case "clientid": options.ClientId = Value().Trim(); break;
                case "user": case "username": options.User = Value(); break;
                case "site": case "s": case "url": options.Site = Value(); break;
                case "library": case "list": case "listguid": options.Library = Value(); break;
                case "local": case "source": options.Local = Value(); break;
                case "folder": case "initialdir": options.Folder = Value(); break;
                case "download": case "reverse": options.Download = true; break;
                case "direction":
                    string direction = Value().ToLowerInvariant();
                    if (direction is not ("upload" or "download")) throw new ArgumentException("--direction must be upload or download.");
                    options.Download = direction == "download";
                    break;
                case "since-last-run": options.SinceLastRun = true; break;
                case "since":
                    if (!DateTimeOffset.TryParse(Value(), System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AssumeUniversal |
                        System.Globalization.DateTimeStyles.AdjustToUniversal, out var cutoff))
                        throw new ArgumentException("--since requires a date, such as 2026-10-06T06:00:00Z. Dates without an offset use UTC.");
                    options.SinceUtc = cutoff.UtcDateTime;
                    break;
                case "delete": options.Delete = true; break;
                case "dry-run": options.DryRun = true; break;
                case "authcheck": options.AuthCheck = true; break;
                case "configfile": options.ConfigFile = Value(); break;
                case "mode":
                    if (Value().ToLowerInvariant() is not ("1" or "config" or "configfile"))
                        throw new ArgumentException("Legacy modes were removed. Use --download or --configfile instead.");
                    break;
                case "confirm":
                    if (Value().ToLowerInvariant() is not ("no" or "n"))
                        throw new ArgumentException("Interactive copy confirmations were removed. Use --dry-run to preview changes.");
                    break;
                case "p": case "pass": case "password":
                    Value();
                    Console.Error.WriteLine("Password arguments are ignored; sign in with the device code.");
                    break;
                default: throw new ArgumentException("Unknown/retired option: " + args[i] + ". Use --help for the sync options.");
            }
        }
        if (options.LocalDateMode && (options.Download || options.AuthCheck))
            throw new ArgumentException("--since-last-run and --since apply to uploads only.");
        if (options.Delete && options.Download)
            throw new ArgumentException("--delete applies to SharePoint in upload mode only. Downloads never delete local files.");
        if (options.AuthCheck)
        {
            if (string.IsNullOrWhiteSpace(options.Site)) throw new ArgumentException("--authcheck requires --site.");
            if (options.Delete || options.Download || options.ConfigFile != "") throw new ArgumentException("Use --authcheck with a site only; no sync/delete/configfile options.");
        }
        else if (options.ConfigFile == "") ValidateJob(new(options.Site, options.Library, options.Local, options.Folder, options.User));
        else if (options.Site != "" || options.Library != "" || options.Local != "" || options.Folder != "")
            throw new ArgumentException("Use either --configfile or --site/--library/--local/--folder.");
        return options;
    }

    public SyncJob[] Jobs()
    {
        if (ConfigFile == "") return [new(Site, Library, Local, Folder, User)];
        using var input = System.IO.File.OpenRead(ConfigFile);
        var database = (MigrationsDatabase)new XmlSerializer(typeof(MigrationsDatabase)).Deserialize(input);
        if (database.Migrations.Count == 0) throw new ArgumentException("The configuration file contains no migrations.");
        if (database.MetadataMigrations.Count != 0)
            throw new ArgumentException("Metadata/custom migrations are no longer supported. Use document sync jobs only.");
        var jobs = database.Migrations.Select(m =>
        {
            if (!string.IsNullOrEmpty(m.Resume) || !string.IsNullOrEmpty(m.ExcludeFiles) || !string.IsNullOrEmpty(m.ExcludeFolders))
                throw new ArgumentException("Legacy resume/exclusion settings are not supported by full-tree sync. Remove these settings before syncing.");
            return new SyncJob(m.SharepointSite, string.IsNullOrWhiteSpace(m.DocLibraryGUiD) ? m.DocLibraryName : m.DocLibraryGUiD,
                m.DocSource, m.StartFolder ?? "", string.IsNullOrWhiteSpace(User) ? m.Username : User);
        }).ToArray();
        foreach (var job in jobs) ValidateJob(job);
        return jobs;
    }

    private static void ValidateJob(SyncJob job)
    {
        if (string.IsNullOrWhiteSpace(job.Site) || string.IsNullOrWhiteSpace(job.Library) || string.IsNullOrWhiteSpace(job.Local))
            throw new ArgumentException("Specify --site, --library and --local (or --configfile). Use --help for examples.");
        if (!string.IsNullOrEmpty(job.Folder)) SyncPaths.Validate(job.Folder);
    }

    public const string Usage = """
        Camulos SharePoint Sync (.NET 10)
        Upload local files/folders that are missing or newer in SharePoint:
          dotnet CamulosSharePointUpload.dll --site URL --library "Documents" --local /srv/docs --tenant TENANT --clientid APP
        Upload by local date without comparing/preserving SharePoint dates:
          add --since-last-run (checkpoint after successful runs; first run selects all)
          optional --since 2026-10-06T06:00:00Z to set/override the cutoff
          selected local files overwrite matching SharePoint content, even if newer remotely
        Download files/folders that are missing or newer locally:
          add --download (or --direction download)
        Preview without writing files, folders or metadata:
          add --dry-run
        Recycle SharePoint files/folders absent locally after successful uploads:
          add --delete (upload only; never deletes the library or selected root)
        Optional: --folder "Subfolder" (existing path inside the library), --user ACCOUNT
        Credentials: --tenant / --clientid or CAMULOS_TENANT_ID / CAMULOS_CLIENT_ID
        Connection check: --site URL --tenant TENANT --clientid APP --authcheck
        Existing XML jobs: --configfile /srv/jobs.xml (same direction/dry-run/delete options)
        Default mode compares UTC timestamps at one-second resolution and skips destination-newer files.
        Local-date uploads use an inclusive UTC cutoff; dry runs/failures never advance the checkpoint.
        Downloading does not remove local extras. Symlinks and case-conflicting names are rejected.
        Cordner/CSV/custom/metadata modes have been removed. See README.md and AUTHENTICATION.md.
        """;
}

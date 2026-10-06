# SharePoint Online authentication

The terminal uploader uses Microsoft Entra ID device-code authentication through
MSAL.NET. It prints a Microsoft sign-in URL and a short code. Open the URL in any
browser, enter the code, and sign in with your work account, including MFA where
required. No browser is needed on the Linux host.

This is an interactive method. Someone must complete sign-in when requested;
`--dry-run` previews sync operations but still needs sign-in to read the library.
Unattended scheduling will need a separate certificate-based application flow.

## One-time Entra setup

1. In Microsoft Entra admin center, open **App registrations > New registration**.
   Register an application for accounts in your organisation only.
2. Record the **Application (client) ID** and **Directory (tenant) ID**.
3. Under **Authentication > Advanced settings**, enable **Allow public client
   flows**. Device-code authentication does not require a client secret or a
   browser redirect on this host.
4. Under **API permissions > Add a permission > SharePoint > Delegated
   permissions**, add **AllSites.Write** for uploading to existing libraries.
   Features that manage lists/sites can require **AllSites.Manage**; features
   requiring full control need the corresponding permission. Grant consent
   according to your organisation's policy (administrator consent if required).
   These are SharePoint permissions, not Microsoft Graph permissions.
5. The signed-in user must also have access to the target site/library. Delegated
   authentication does not give the user access they do not already have.

Tenant Conditional Access can block device-code sign-in. If your policy blocks
it, ask your administrator which permitted flow to use.

## Connection check (no uploads)

Once the project builds with the .NET 10 SDK:

```bash
dotnet run --project CamulosSharePointUpload.csproj -- \
  -tenant "YOUR-TENANT-ID" \
  -clientid "YOUR-CLIENT-ID" \
  -site "https://example.sharepoint.com/sites/upload" \
  -authcheck
```

Complete the browser sign-in. Success prints the site's title and URL. Failure
prints an error and returns exit code 1. This command only reads site information.
`-user "user@example.com"` is optional; if supplied, the signed-in account must
match that username.

## Sync / existing configuration files

Use `--tenant` and `--clientid` with the sync command, or set the environment
variables below. See [README.md](README.md) for upload, download, dry-run and
remote-delete examples.

```bash
export CAMULOS_TENANT_ID="YOUR-TENANT-ID"
export CAMULOS_CLIENT_ID="YOUR-CLIENT-ID"
dotnet CamulosSharePointUpload.dll --configfile "/srv/migrations/jobs.xml"
```

Command-line tenant/client settings override environment settings and apply to
all document entries in a configuration file. The existing XML format is
preserved; its `Username` is an optional account selection hint and `Password`
is ignored. Unsupported metadata/resume/exclusion jobs are rejected before sync.
Legacy password command-line options are ignored with a message. There is no
fallback to username/password authentication.

MSAL keeps tokens only in memory for this process, reuses them across contexts,
and renews them silently where possible. It requests tokens for each SharePoint
host separately when a batch includes different sites. A new process normally
requires a new device-code sign-in. No tokens or passwords are written to disk.
If renewal requires user interaction, another device code is displayed.

Transfers use the normal CSOM request pipeline with bearer tokens. Large uploads
use chunked CSOM operations; downloads stream to temporary local files. Windows
OLE DB, file ownership/Office metadata extraction and legacy migration paths have
been removed. Live SharePoint validation is still required.

References:
- https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/desktop-mobile/device-code-flow
- https://learn.microsoft.com/en-us/sharepoint/dev/sp-add-ins/using-csom-for-dotnet-standard

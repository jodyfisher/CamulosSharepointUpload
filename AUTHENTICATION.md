# SharePoint Online authentication

The terminal uploader uses Microsoft Entra ID device-code authentication through
MSAL.NET. It prints a Microsoft sign-in URL and a short code. Open the URL in any
browser, enter the code, and sign in with your work account, including MFA where
required. No browser is needed on the Linux host.

This is an interactive method. Someone must complete sign-in when requested;
`-confirm no` suppresses upload confirmations but does not suppress sign-in.
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

## Upload / existing configuration files

Add `-tenant` and `-clientid` to the existing upload arguments, and remove password
arguments. Alternatively, set these environment variables in your shell:

```bash
export CAMULOS_TENANT_ID="YOUR-TENANT-ID"
export CAMULOS_CLIENT_ID="YOUR-CLIENT-ID"
dotnet CamulosSharePointUpload.dll -mode config -configfile "/srv/migrations/jobs.xml"
```

Command-line tenant/client settings override environment settings and apply to
all entries in that configuration file. The existing XML format is preserved;
its `Username` is an optional account selection hint and its `Password` is ignored.
Legacy password command-line options are ignored with a message. There is no
fallback to username/password authentication.

MSAL keeps tokens only in memory for this process, reuses them across contexts,
and renews them silently where possible. It requests tokens for each SharePoint
host separately (including source and destination sites). A new process normally
requires a new device-code sign-in. No tokens or passwords are written to disk.
If renewal requires user interaction, another device code is displayed.

The old WebDAV/BinaryDirect calls have been replaced with CSOM stream operations
so transfers use the same bearer-token authentication as metadata requests.
Large-file/chunked transfer behaviour still needs live validation.

## Current Linux migration limits

Authentication is implemented, but the entire uploader has not been validated on
Linux. CSV loading still uses Windows OLE DB, and legacy path/ownership handling
needs further migration. The OLE DB package is retained only to allow those
existing code paths to compile; it cannot read CSV files on Linux.

References:
- https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/desktop-mobile/device-code-flow
- https://learn.microsoft.com/en-us/sharepoint/dev/sp-add-ins/using-csom-for-dotnet-standard

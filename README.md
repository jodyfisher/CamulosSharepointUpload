# Camulos SharePoint Sync

A .NET 10 terminal tool for syncing a local directory with a SharePoint Online
document library. Works on Linux and uses Microsoft Entra device-code sign-in.
See [AUTHENTICATION.md](AUTHENTICATION.md) for the one-time app registration.

## Build and run

```bash
dotnet build CamulosSharePointUpload.csproj -c Release
export CAMULOS_TENANT_ID="YOUR-TENANT-ID"
export CAMULOS_CLIENT_ID="YOUR-CLIENT-ID"
```

Upload documents and folder structure:

```bash
dotnet bin/Release/net10.0/CamulosSharePointUpload.dll \
  --site "https://example.sharepoint.com/sites/team" \
  --library "Documents" --local "/srv/documents" --dry-run
```

Remove `--dry-run` to perform the sync. Sign in using the URL/code printed in the
terminal. `--library` accepts the library's title or GUID. `--folder "Projects"`
restricts the sync to an existing subfolder inside that library. The library and
selected root folder must exist; children are created as needed, including empty
folders. A library URL is not the same as its display title.

## Timestamp behaviour

Upload is the default. A file is copied when it is missing remotely or its local
UTC modified time is newer than SharePoint's `Modified` time. Files with equal
or newer destination timestamps are left alone. Comparison uses whole seconds
because SharePoint can round timestamps. Equal timestamps with differing content
or sizes are still skipped; this is a timestamp sync, not a checksum comparison.

After upload, the tool sets SharePoint's `Modified` timestamp to the local source
mtime (at whole-second resolution) and verifies size plus the library item's
`Modified` field in a separate read after updating it. Timestamp assignment
uses the original item-update then file-update sequence; this may create an
additional metadata version in a versioned library. Scan, overwrite protection
and verification all read that same field using explicit UTC CAML queries,
independent of the site's regional time zone. A verification error reports
expected and actual values. The account must be permitted to update that metadata. Other metadata/permissions are not copied from the local file.

Individual file/folder errors are printed and appended to `Errors.txt` in the
current working directory, with UTC timestamps and relative paths. The run
continues with the remaining entries; a failed folder creation skips descendants
of that folder. Any item error cancels the remote deletion phase and the final
exit code is 1. Scanning, authentication and library setup failures still stop
the run, because a complete sync plan cannot be obtained.

Previously completed operations remain; this is not a transaction. Avoid editing
either tree during a sync. The tool checks scanned files before using them,
verifies transfers, and checks the local source again before remote deletion,
but it cannot provide an atomic snapshot against simultaneous edits.

## Remote deletion (`--delete`)

Add `--delete` to an upload command to recycle remote files/folders that have no
matching relative path in the local tree. Preview first with `--delete --dry-run`.
The flag is a one-way mirror of names, not a way to overwrite remote-newer files.

- Deletion is limited to the selected library/subfolder; its root is retained.
- SharePoint's system `Forms` folder is protected.
- Remote extras are moved to SharePoint's recycle bin, not permanently erased.
- Folders are recycled deepest-first and only after checking that they are empty.
- Deletion starts only after complete scans and successful transfers. A changed
  local source tree cancels deletion. A missing/unreadable source never acts as
  an empty tree. An intentionally empty, readable source with `--delete` will
  recycle all document content within the selected scope.

## Download newer files (reverse direction)

Use the same command with `--download` (or `--direction download` / `--reverse`):

```bash
dotnet bin/Release/net10.0/CamulosSharePointUpload.dll \
  --site "https://example.sharepoint.com/sites/team" \
  --library "Documents" --folder "Projects" \
  --local "/srv/documents" --download --dry-run
```

Missing local files and files newer in SharePoint are downloaded. Local-newer
files and local extras are retained. Downloads use a temporary file in the target
directory and replace the destination only after the full download is received;
the SharePoint modified timestamp is preserved locally. `--download --delete`
is rejected: local deletion is deliberately not supported.

## Paths and large files

Git metadata entries named `.git` (including their descendants), Windows
download metadata ending in `:Zone.Identifier` or `:Zone.Identifier:$DATA`,
macOS `.DS_Store` / `._*` AppleDouble files, and Windows `Thumbs.db` /
`desktop.ini` are excluded automatically, in both directions. The actual document is still
synced. These exclusions also protect existing remote entries from `--delete`;
ancestor folders containing excluded content are retained. Existing uploaded
`.git` content needs manual cleanup if you want to remove it from SharePoint.
Local files are never deleted by these exclusions.

Local names are mapped to SharePoint names by replacing unsupported characters
(`" * : < > ? \ |` and control characters) with spaces, trimming leading/trailing
spaces. An entirely empty name becomes `_`. Supported characters such as `&`,
commas, `#` and `%` are preserved (subject to tenant settings for `#` / `%`).
Original local names are retained on disk. For example, `Invoice:2025.pdf` maps
to `Invoice 2025.pdf` in SharePoint.

The same mapped names are used for comparisons and deletion. Downloads reuse
an existing local file's original name when it maps to the SharePoint name;
new downloads use the SharePoint name because the replacement is not reversible.
If two local entries map to the same name, the scan stops before any writes or
deletion, reporting both paths. Before uploading, reserved names and paths over
400 characters are logged and skipped, while valid entries continue.


Relative directory structure is preserved. Decoded SharePoint ResourcePath APIs
support names containing spaces, `%` and `#`. Linux case-conflicting names
(`File.txt` and `file.txt`) and symbolic links are rejected before transfer rather
than producing an incomplete mirror. Existing Linux folder/name casing is reused
when it differs from SharePoint only by case.

Files over 8 MiB use bounded CSOM upload chunks. New large uploads stage under a
unique `.camulos-upload-*` name and move into place when complete. An interrupted
process can leave a temporary remote item; `--delete` can recycle it on a later
successful run. Required checkout, retention policies, locks, permissions, list
view thresholds and very large libraries/files still require live testing in
your tenant. The tool does not bypass SharePoint library policies.

## Existing XML jobs and retired features

`--configfile "/srv/jobs.xml"` still reads existing document `Migration` entries.
Tenant/client settings are supplied as usual. The command's direction, dry-run
and delete settings apply to every entry. Only the document source, site,
library/GUID, start folder and optional username are used. Passwords are ignored.
Legacy metadata jobs and nonempty resume/exclusion settings are rejected before
running the batch, since they do not represent a complete mirror.

Common aliases such as `-site`, `/site`, `-list`, `-source`, `-initialdir`,
`-tenant`, `-clientid`, and `-mode config -configfile ...` still work.
`-confirm no` is accepted for existing scripts; copy confirmations are replaced
by `--dry-run`. Old overwrite/date-filter/logging and custom options are rejected
with a clear error; timestamps now determine which files get copied.

Windows Forms, Cordner, CSV/OLE DB, SharePoint-to-SharePoint/custom-script and
metadata migration workflows have been removed. XML data model classes remain
only to read the existing document job format.

## Validation

```bash
dotnet run --project tests/CamulosSharePointUpload.Tests.csproj -c Release
```

Tests cover timestamp/direction decisions, dry runs, delete ordering, prevention
of deletion after failures/source changes, path/case/link handling and local
atomic downloads. Live SharePoint authentication, paging, uploads, metadata
stamping and recycling still need validation using your tenant/app registration.
Run `--authcheck` first, then a dry run against a small test library.

The existing licence text is retained in [readme.txt](readme.txt).

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

## Upload files changed since the previous run

Add `--since-last-run` to use local-date uploads without relying on SharePoint
preserving the source date:

```bash
dotnet bin/Release/net10.0/CamulosSharePointUpload.dll \
  --site "https://example.sharepoint.com/sites/team" \
  --library "Documents" --local "/srv/documents" \
  --since-last-run --dry-run
```

Remove `--dry-run` to upload. A separate checkpoint is saved for each tenant,
SharePoint scope and absolute local directory, under the running user's local
application-data directory (`CamulosSharePointUpload/checkpoints`). On Linux this
normally sits under `~/.local/share` or `XDG_DATA_HOME`. The command prints its path.
Changing the current working directory does not reset the checkpoint.

With `--since-last-run`, verified uploads are recorded immediately in a sibling
`<checkpoint>.progress.jsonl` file and flushed to disk after each file. If you
press Ctrl+C or the process stops, the next run with the same cutoff skips
recorded files whose local modified time and size are unchanged, provided the
remote file still exists with the expected size. Files changed locally, missing
remotely or with a different remote size are uploaded again. This uses metadata,
not content hashes; same-size remote edits are not detected by the resume record.
The file in progress when interrupted may need to be uploaded again.

The datetime cutoff is not advanced mid-run. Completed runs save it and clear
the resume record, including runs with logged individual failures. An explicit
`--since` that changes the cutoff starts a fresh resume record. Dry runs can
preview existing progress but do not create, change or clear it. `--since` alone
does not save resume progress; keep `--since-last-run` to enable it.

- Files are selected when their local UTC modified time is **at or after** the
  checkpoint, including missing files only if their timestamps meet that cutoff.
- With no checkpoint, the first run selects all local files. Use
  `--since-last-run --since "2026-10-06T06:00:00Z"` to choose an initial cutoff
  or override a previous checkpoint for that run.
- Selected files overwrite matching remote content **even when SharePoint's
  timestamp is newer**. Use this mode when the local directory is authoritative.
- SharePoint keeps its upload timestamp; this mode does not attempt to preserve
  source dates. The uploaded size is verified. The default timestamp mode remains
  available by omitting `--since-last-run` and `--since`.
- Every completed run advances the checkpoint to that run's **start** time,
  even if individual files/folders failed. These failures are logged in
  `Errors.txt` and do not make the next run repeat the whole upload.
  Failed entries whose timestamps fall before the new cutoff are not retried
  automatically; use an earlier `--since` cutoff to retry them.
  Changes made during a run remain eligible next time. The saved start time is
  rounded down to whole seconds to accommodate filesystem timestamp precision.
  Dry runs and interrupted/stopped runs do not advance the checkpoint.
- Files restored or newly added with old modified dates are outside the cutoff.
  An explicit earlier `--since` date can select them again.
- Existing folders are retained and missing folders are created, including empty
  ones. If `--delete` is used, it still compares the **complete** local tree, so
  unchanged older local files are not mistaken for remote extras.
- This mode is upload-only. `--download` retains the existing timestamp rules.

To run once against a chosen date without saving a checkpoint, use
`--since "2026-10-06T06:00:00Z"` alone. Dates without an explicit offset are
interpreted as UTC. A second simultaneous run for the same checkpoint is rejected
before writes. Keep the checkpoint files if you move or redeploy the executable.

## Timestamp behaviour

Upload is the default. A file is copied when it is missing remotely or its local
UTC modified time is newer than SharePoint's `Modified` time. Files with equal
or newer destination timestamps are left alone. Comparison uses whole seconds
because SharePoint can round timestamps. Equal timestamps with differing content
or sizes are still skipped; this is a timestamp sync, not a checksum comparison.

In default timestamp mode, the tool attempts to set SharePoint's `Modified` timestamp to the local source
mtime (at whole-second resolution) and verifies size plus the library item's
`Modified` field in a separate read after updating it. Timestamp assignment
uses SharePoint's post-upload `ValidateUpdateListItem` API with
`bNewDocumentUpdate=true`, submitting `Modified` together with the existing
`Editor`. The source UTC time is sent in the invariant form-value date format
with `datesInUTC=true`; no client/site local-time conversion is performed.
Field validation errors are logged. A readback mismatch includes the submitted
mode and the per-field validation response for diagnosis. Scan, overwrite protection and verification all read that
same field using explicit UTC CAML queries,
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
The flag controls a one-way mirror of names. Transfer/overwrite decisions follow the selected upload mode.

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
`desktop.ini` are excluded automatically, in both directions. Folders named
`bin` or `obj` (case-insensitively) and their entire contents are also excluded;
files merely named `bin` or `obj` are retained. The actual document is still
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

Tests cover interruption/resume progress, build-folder exclusions,
timestamp/direction decisions, dry runs, delete ordering, prevention
of deletion after failures/source changes, path/case/link handling and local
atomic downloads. Live SharePoint authentication, paging, uploads, metadata
stamping and recycling still need validation using your tenant/app registration.
Run `--authcheck` first, then a dry run against a small test library.

The existing licence text is retained in [readme.txt](readme.txt).

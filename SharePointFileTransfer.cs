using Microsoft.SharePoint.Client;
using SPFile = Microsoft.SharePoint.Client.File;

namespace CamulosSharePointUpload;

internal static class SharePointFileTransfer
{
    private const int ChunkSize = 8 * 1024 * 1024;

    public static SPFile Upload(ClientContext context, Folder folder, string url, Stream content, long length, bool overwrite)
    {
        var parameters = new FileCollectionAddParameters { Overwrite = overwrite };
        if (length <= ChunkSize)
        {
            var smallBytes = new byte[(int)length];
            content.ReadExactly(smallBytes, 0, smallBytes.Length);
            if (content.ReadByte() != -1) throw new IOException("Source size changed during upload: " + url);
            using var buffer = new MemoryStream(smallBytes, false);
            var small = folder.Files.AddUsingPath(ResourcePath.FromDecodedUrl(url), parameters, buffer);
            context.ExecuteQuery();
            return small;
        }

        Guid uploadId = Guid.NewGuid();
        // Existing content stays in place until the upload session finishes. New large
        // files use a temporary name so an interrupted session cannot masquerade as a
        // newer completed destination on the next run.
        string uploadUrl = overwrite ? url : url[..(url.LastIndexOf('/') + 1)] + ".camulos-upload-" + uploadId.ToString("N");
        SPFile file;
        if (overwrite) file = context.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(url));
        else
        {
            using var empty = new MemoryStream();
            file = folder.Files.AddUsingPath(ResourcePath.FromDecodedUrl(uploadUrl), parameters, empty);
            context.ExecuteQuery();
        }
        long offset = 0;
        var bytes = new byte[ChunkSize];
        try
        {
            while (offset < length)
            {
                int count = (int)Math.Min(bytes.Length, length - offset);
                content.ReadExactly(bytes, 0, count);
                using var chunk = new MemoryStream(bytes, 0, count, false);
                if (offset == 0)
                {
                    var result = file.StartUpload(uploadId, chunk);
                    context.ExecuteQuery();
                    if (result.Value != offset + count) throw new IOException("Unexpected upload offset: " + url);
                    offset = result.Value;
                }
                else if (offset + count == length)
                {
                    file = file.FinishUpload(uploadId, offset, chunk);
                    context.ExecuteQuery();
                    offset += count;
                }
                else
                {
                    var result = file.ContinueUpload(uploadId, offset, chunk);
                    context.ExecuteQuery();
                    if (result.Value != offset + count) throw new IOException("Unexpected upload offset: " + url);
                    offset = result.Value;
                }
            }
            if (content.ReadByte() != -1) throw new IOException("Source grew during upload: " + url);
            if (!overwrite)
            {
                file.MoveToUsingPath(ResourcePath.FromDecodedUrl(url), MoveOperations.None);
                context.ExecuteQuery();
                file = context.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(url));
            }
            return file;
        }
        catch
        {
            try { file.CancelUpload(uploadId); context.ExecuteQuery(); } catch { }
            if (!overwrite)
            {
                try
                {
                    context.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(uploadUrl)).Recycle();
                    context.ExecuteQuery();
                }
                catch { } // A terminated process can leave a temporary file; --delete recycles extras.
            }
            throw;
        }
    }
}

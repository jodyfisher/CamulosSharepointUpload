using Microsoft.SharePoint.Client;

namespace CamulosSharePointUpload
{
    internal static class SharePointFileTransfer
    {
        // Uses the normal CSOM request pipeline, including its bearer-token handler.
        public static void Upload(ClientContext context, string serverRelativeUrl, Stream content, bool overwrite)
        {
            int separator = serverRelativeUrl.LastIndexOf('/');
            if (separator < 0 || separator == serverRelativeUrl.Length - 1)
                throw new ArgumentException("A server-relative file URL is required.", nameof(serverRelativeUrl));
            string folderUrl = separator == 0 ? "/" : serverRelativeUrl.Substring(0, separator);
            var folder = context.Web.GetFolderByServerRelativeUrl(folderUrl);
            folder.Files.Add(new FileCreationInformation
            {
                Url = serverRelativeUrl.Substring(separator + 1),
                ContentStream = content,
                Overwrite = overwrite
            });
            context.ExecuteQuery();
        }

        public static void Copy(ClientContext source, ClientContext target, string sourceUrl, string targetUrl, bool overwrite)
        {
            var file = source.Web.GetFileByServerRelativeUrl(sourceUrl);
            var result = file.OpenBinaryStream();
            source.ExecuteQuery();
            using (var stream = result.Value)
            {
                Upload(target, targetUrl, stream, overwrite);
            }
        }
    }
}

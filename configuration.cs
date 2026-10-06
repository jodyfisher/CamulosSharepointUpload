using Microsoft.SharePoint.Client;

namespace CamulosSharePointUpload;

internal static class Configuration
{
    public static string tenantId = "";
    public static string clientId = "";
    public static string o365UserName = "";
    public static ClientContext GetUserContext(string siteURL) => SharePointAuthentication.CreateContext(siteURL);
}

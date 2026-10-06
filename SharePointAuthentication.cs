using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;

namespace CamulosSharePointUpload
{
    // One MSAL cache per tenant/app for this process. Tokens are never written to disk.
    internal static class SharePointAuthentication
    {
        private static readonly object sync = new object();
        private static IPublicClientApplication application;
        private static string applicationKey;

        public static void ValidateSettings()
        {
            if (!Guid.TryParse(Configuration.clientId, out _))
                throw new ArgumentException("Specify the Entra application client ID with -clientid or CAMULOS_CLIENT_ID.");
            string tenant = Configuration.tenantId;
            if (string.IsNullOrWhiteSpace(tenant) ||
                (!Guid.TryParse(tenant, out _) && Uri.CheckHostName(tenant) != UriHostNameType.Dns) ||
                tenant.Equals("common", StringComparison.OrdinalIgnoreCase) ||
                tenant.Equals("organizations", StringComparison.OrdinalIgnoreCase) ||
                tenant.Equals("consumers", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Specify your tenant ID or verified tenant domain with -tenant or CAMULOS_TENANT_ID.");
        }

        public static ClientContext CreateContext(string siteUrl)
        {
            ValidateSettings();
            if (!Uri.TryCreate(siteUrl, UriKind.Absolute, out Uri site) || site.Scheme != Uri.UriSchemeHttps ||
                !string.IsNullOrEmpty(site.UserInfo) || !site.IsDefaultPort ||
                !site.Host.EndsWith(".sharepoint.com", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Specify an HTTPS SharePoint Online site URL, for example https://example.sharepoint.com/sites/upload.");

            string resource = site.GetLeftPart(UriPartial.Authority);
            string tenant = Configuration.tenantId;
            string clientId = Configuration.clientId;
            string user = Configuration.o365UserName;
            var context = new ClientContext(site.AbsoluteUri);
            // Obtain a token on every request through MSAL; its cache handles expiry and renewal.
            context.ExecutingWebRequest += (_, e) =>
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + GetToken(resource, tenant, clientId, user);
            };
            return context;
        }

        private static string GetToken(string resource, string tenant, string clientId, string user)
        {
            lock (sync)
            {
                string key = tenant + ":" + clientId;
                if (application == null || applicationKey != key)
                {
                    application = PublicClientApplicationBuilder.Create(clientId)
                        .WithAuthority("https://login.microsoftonline.com/" + tenant)
                        .Build();
                    applicationKey = key;
                }

                string[] scopes = { resource + "/.default" };
                var accounts = application.GetAccountsAsync().GetAwaiter().GetResult();
                var account = string.IsNullOrWhiteSpace(user)
                    ? accounts.FirstOrDefault()
                    : accounts.FirstOrDefault(a => string.Equals(a.Username, user, StringComparison.OrdinalIgnoreCase));
                try
                {
                    return application.AcquireTokenSilent(scopes, account)
                        .ExecuteAsync().GetAwaiter().GetResult().AccessToken;
                }
                catch (MsalUiRequiredException)
                {
                    var result = application.AcquireTokenWithDeviceCode(scopes, code =>
                    {
                        Console.WriteLine(code.Message);
                        if (!string.IsNullOrWhiteSpace(user))
                            Console.WriteLine("Sign in as: " + user);
                        return Task.CompletedTask;
                    }).ExecuteAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrWhiteSpace(user) &&
                        !string.Equals(result.Account.Username, user, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("The signed-in account differs from -user/configuration Username. Use the correct account or omit the username hint.");
                    return result.AccessToken;
                }
            }
        }
    }
}

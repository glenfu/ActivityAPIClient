namespace ActivityApiClient
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Microsoft.Identity.Client;

    class AADHelper
    {
        //private static string AuthorityUrl = ConfigurationManager.AppSettings["AAD:AuthorityUrl"];
        private static string ClientId = ConfigurationManager.AppSettings["AAD:ClientId"];
        private static string ClientKey = ConfigurationManager.AppSettings["AAD:ClientKey"];
        //private static Uri ClientRedirectUri = new Uri(ConfigurationManager.AppSettings["AAD:ClientRedirectUri"]);
        private static string ResourceID = ConfigurationManager.AppSettings["AAD:ResourceID"];
        private static IConfidentialClientApplication app;

        /// <summary>
        /// Retrieves an access token from AAD using the client app credentials.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static async Task<string> AuthenticateAndGetToken(string tenantId)
        {
            AuthenticationResult authResult = null;

            try
            {
                if (app == null)
                {
                    app = ConfidentialClientApplicationBuilder.Create(ClientId)
                        .WithClientSecret(ClientKey)
                        .Build();

                }

                authResult = await app.AcquireTokenForClient(
                new[] { AADHelper.ResourceID })
                .WithTenantId(tenantId)
                // See https://aka.ms/msal.net/withTenantId
                .ExecuteAsync()
                .ConfigureAwait(false);

                if (authResult == null)
                {
                    Console.WriteLine("A token was not received from AAD.");
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "Inner Exception : " + ex.InnerException.Message;
                }

                Console.WriteLine("There was an exception when requesting a token from AAD.");
                Console.WriteLine(message);
            }
            return authResult.AccessToken;
        }
    }
}

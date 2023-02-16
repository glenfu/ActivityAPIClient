namespace ActivityApiClient
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    class AADHelper
    {
        private static string AuthorityUrl = ConfigurationManager.AppSettings["AAD:AuthorityUrl"];
        private static string ClientId = ConfigurationManager.AppSettings["AAD:ClientId"];
        private static string ClientKey = ConfigurationManager.AppSettings["AAD:ClientKey"];
        //private static Uri ClientRedirectUri = new Uri(ConfigurationManager.AppSettings["AAD:ClientRedirectUri"]);
        private static string ResourceID = ConfigurationManager.AppSettings["AAD:ResourceID"];

        /// <summary>
        /// Retrieves an access token from AAD using the client app credentials.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static async Task<string> AuthenticateAndGetToken(string tenantId)
        {
            AuthenticationContext authContext = new AuthenticationContext(String.Format(AuthorityUrl, tenantId));
            ClientCredential clientCredentials = new ClientCredential(AADHelper.ClientId, AADHelper.ClientKey);

            AuthenticationResult result = null;
            string accessToken;

            try
            {
                result = await authContext.AcquireTokenAsync(AADHelper.ResourceID, clientCredentials);
                accessToken = result.AccessToken;

                if (String.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("A token was not received from AAD.");
                }
            }
            catch (AdalException ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "Inner Exception : " + ex.InnerException.Message;
                }

                Console.WriteLine("There was an exception when requesting a token from AAD.");
                Console.WriteLine(message);
            }

            return result.AccessToken;
        }
    }
}

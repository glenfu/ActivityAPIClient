
namespace ActivityApiClient
{
    using System.Collections.Generic;
    using System.Configuration;

    class RequestContext
    {
        public string TenantId;
        public string ContentId;
        public string ContentType;
        public string ContentUrl;
        public string WebhookAddress;
        public string WebhookAuthId;
        public string WebhookExpiration;
        public string StartTime;
        public string EndTime;
        public string Token;

        public RequestContext(Dictionary<string, string> parameters)
        {
            // default values from configuration
            TenantId = ConfigurationManager.AppSettings["TenantId"];
            WebhookAddress = ConfigurationManager.AppSettings["WebhookAddress"];
            WebhookAuthId = ConfigurationManager.AppSettings["WebhookAuthId"];
            WebhookExpiration = ConfigurationManager.AppSettings["WebhookExpiration"];

            // parsed parameters
            TenantId = parameters.ContainsKey("tenantid") ? parameters["tenantid"] : TenantId;
            ContentId = parameters.ContainsKey("contentid") ? parameters["contentid"] : "";
            ContentType = parameters.ContainsKey("contenttype") ? parameters["contenttype"] : "";
            ContentUrl = parameters.ContainsKey("contenturl") ? parameters["contenturl"] : "";
            WebhookAddress = parameters.ContainsKey("webhookaddress") ? parameters["webhookaddress"] : WebhookAddress;
            WebhookAuthId = parameters.ContainsKey("webhookauthid") ? parameters["webhookauthid"] : WebhookAuthId;
            WebhookExpiration = parameters.ContainsKey("webhookexpiration") ? parameters["webhookexpiration"] : WebhookExpiration;
            StartTime = parameters.ContainsKey("starttime") ? parameters["starttime"] : "";
            EndTime = parameters.ContainsKey("endtime") ? parameters["endtime"] : "";
            Token = parameters.ContainsKey("token") ? parameters["token"] : "";
        }
    }
}

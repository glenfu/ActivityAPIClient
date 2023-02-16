using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using ActivityApiClient;

namespace ActivityAPIClient
{
    class Program
    {
        private static string ActivityApiBaseUrl;
        private static string AccessToken;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            RequestContext context;
            string Command = ParseArgs(args, out context);

            AccessToken = (string.IsNullOrEmpty(context.Token)) ? AADHelper.AuthenticateAndGetToken(context.TenantId).Result : context.Token;

            if (String.IsNullOrEmpty(AccessToken))
            {
                return;
            }

            ActivityApiBaseUrl = String.Format(ConfigurationManager.AppSettings["ActivityApiBaseUrl"], context.TenantId);

            switch (Command)
            {
                case "start":
                    StartSubscription(context).Wait();
                    break;

                case "stop":
                    StopSubscription(context).Wait();
                    break;

                case "subscriptions":
                    ListSubscriptions(context).Wait();
                    break;

                case "listcontent":
                    ListAvailableContent(context).Wait();
                    break;

                case "getcontent":
                    GetContent(context).Wait();
                    break;

                case "notifications":
                    GetNotifications(context).Wait();
                    break;

                case "token":
                    Console.WriteLine("{0}\n", AccessToken);
                    break;

                case "help":
                    PrintHelp();
                    break;

                default:
                    Console.WriteLine("Unknown Command: " + Command);
                    PrintHelp();
                    break;
            }
        }

        static string ParseArgs(string[] args, out RequestContext context)
        {
            string command = args[0];

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i += 2)
                {
                    parameters.Add(args[i].ToLower().Substring(1, args[i].Length - 1), args[i + 1]);
                }
            }

            context = new RequestContext(parameters);

            return command;
        }

        static async Task StartSubscription(RequestContext context)
        {
            StartInput startInput = new StartInput();
            Webhook webhook = new Webhook();
            webhook.address = context.WebhookAddress;
            webhook.authID = context.WebhookAuthId;
            webhook.expiration = context.WebhookExpiration;
            if (string.IsNullOrEmpty(webhook.address + webhook.authID + webhook.expiration))
            {
                startInput.webhook = null;
            }
            else
            {
                startInput.webhook = webhook;
            }

            string url = ActivityApiBaseUrl + "subscriptions/start?contentType=" + context.ContentType;

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            request.Content = SerializeRequestBody(startInput);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}\n", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// Stop a subscription.
        /// </summary>
        static async Task StopSubscription(RequestContext context)
        {
            string url = ActivityApiBaseUrl + "subscriptions/stop?contentType=" + context.ContentType;

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}\n", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// List existing subscriptions.
        /// </summary>
        static async Task ListSubscriptions(RequestContext context)
        {
            string url = ActivityApiBaseUrl + "subscriptions/list";

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// List available content for a subscription.
        /// </summary>
        static async Task ListAvailableContent(RequestContext context)
        {
            string url = ActivityApiBaseUrl + String.Format("subscriptions/content?contentType={0}&endtime={1}&starttime={2}", context.ContentType, context.EndTime, context.StartTime);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}\n", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// List notifications the service has sent for a subscription.
        /// </summary>
        static async Task GetNotifications(RequestContext context)
        {
            string url = ActivityApiBaseUrl + String.Format("subscriptions/notifications?contentType={0}&endtime={1}&starttime={2}", context.ContentType, context.EndTime, context.StartTime);
       
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}\n", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// Retrieve content.
        /// </summary>
        static async Task GetContent(RequestContext context)
        {
            string url = context.ContentUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = ActivityApiBaseUrl + "audit/" + context.ContentId;
            }
            url.Replace("$", "%24");

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage response = await client.SendAsync(request);
            string output = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode == true)
            {
                Console.WriteLine("\n{0}\n", JSONHelper.FormatJson(output));
            }
            else
            {
                Console.WriteLine("\nError:  {0}\n More Details: {1}", response.ReasonPhrase, output);
            }
        }

        /// <summary>
        /// Helper method for JSON serialization.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static StringContent SerializeRequestBody(object input)
        {
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(input.GetType());
            MemoryStream ms = new MemoryStream();
            jsonSer.WriteObject(ms, input);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);
            StringContent content = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");

            return content;
        }


        static void PrintHelp()
        {
            Console.WriteLine(@"
                    Syntax:
                    ActivityApiClient command [-name1 value1] ... [-nameN valueN]
 
                    Default values for some parameters are defined in the configuration file:  TenantId, WebHookAddress, WebHookAuthId, WebhookExpiration
 
                    Start a subscription:
                        ActivityApiClient  start -contenttype ct [-tenantid tid] [-webhookaddress wha] [-webhookauthid whai] [-webhookexpiration e] 
 
                    Stop a subscription :
                        ActivityApiClient stop -contenttype ct
 
                    List existing subscriptions:
                        ActivityApiClient subscriptions [-tenantid tid]
 
                    List available content:
                        ActivityApiClient listcontent -contenttype ct [-tenantid tid] [-starttime st] [-endtime et]
 
                    Retrieve content:
                        ActivityApiClient getcontent -contentid cid | -contenturl curl [-tenantid tid]
 
                    List notifications sent for a subscription:
                        ActivityApiClient notifications -contenttype ct [-tenantid tid] [-starttime st] [-endtime et]
 
                    Retrieve a token from Azure AD:
                        ActivityApiClient token           ");
        }

    }
}

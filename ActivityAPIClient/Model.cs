namespace ActivityApiClient
{
    public partial class Webhook
    {
        public string address { get; set; }
        public string authID { get; set; }
        public string expiration { get; set; }
    }

    public partial class StartInput
    {
        public Webhook webhook { get; set; }
    }
}

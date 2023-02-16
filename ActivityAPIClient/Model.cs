namespace ActivityApiClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

using System;
namespace ar_dashboard.Models.ClientReceiveForm
{
    public class ClientReceiveSignup
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        public ClientReceiveSignup(int _status)
        {
            Status = _status;
        }
    }
}

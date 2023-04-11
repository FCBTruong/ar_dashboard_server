using System;
namespace ar_dashboard.Models.ClientReceiveForm
{
    public class ClientReceiveSignup
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "extra")]
        public string Extra { get; set; }
        
        public ClientReceiveSignup(int _status, string _extra)
        {
            Status = _status;
            Extra = _extra;
        }
    }
}

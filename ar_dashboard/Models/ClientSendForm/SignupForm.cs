using System;
namespace ar_dashboard.Models.ClientSendForm
{
    public class SignupForm
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "password")]
        public string Password { get; set; }


        [Newtonsoft.Json.JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}

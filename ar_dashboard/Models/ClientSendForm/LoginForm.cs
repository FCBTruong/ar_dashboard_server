using System;
namespace ar_dashboard.Models.ClientSendForm
{
    public class LoginForm
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

    }
}

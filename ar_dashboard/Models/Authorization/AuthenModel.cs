using System;

namespace ar_dashboard.Models.Authentication
{
    public class AuthenModel
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role")]
        public ushort Role { get; set; }
    }
}

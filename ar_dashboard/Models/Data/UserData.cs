using System;
using System.Collections.Generic;
using ar_dashboard.Models.Data;

namespace ar_dashboard.Models
{
    public class UserData :ObjectData
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "museums")]
        public List<Museum> Museums { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role")]
        public UserRole Role { get; set; }
    }

    public enum UserRole: ushort
    {
        GUEST = 0,
        USER = 1,
        ADMIN = 2
    }
}

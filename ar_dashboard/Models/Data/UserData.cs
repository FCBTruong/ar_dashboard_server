using System;
using System.Collections.Generic;

namespace ar_dashboard.Models
{
    public class UserData
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "museums")]
        public List<Museum> Museums { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "role")]
        public UserRole Role { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "assets")]
        public List<Asset3D> Assets { get; set; }

        public UserData()
        {
            Name = "default";
            Email = "default";
            Museums = new List<Museum>();
            Role = UserRole.USER;
            Assets = new List<Asset3D>();
        }
    }

    public enum UserRole: ushort
    {
        GUEST = 0,
        USER = 1,
        ADMIN = 2
    }
}

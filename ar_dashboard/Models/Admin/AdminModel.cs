using System;
using System.Collections.Generic;

namespace ar_dashboard.Models.Admin
{
    public class AdminModel
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "publicized_museums")]
        public List<PublicMuseumPost> PublicizedMuseums { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "pending_museums")]
        public List<PublicMuseumPost> PendingMuseums { get; set; }
        public AdminModel()
        {
            PublicizedMuseums = new List<PublicMuseumPost>();
            PendingMuseums = new List<PublicMuseumPost>();
        }
    }


    public class PublicMuseumPost
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "museum")]
        public Museum Museum { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        public PublicMuseumPost()
        {
            Museum = new Museum();
        }
    }
}

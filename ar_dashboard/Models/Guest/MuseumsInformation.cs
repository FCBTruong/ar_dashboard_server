using System;
using System.Collections.Generic;

namespace ar_dashboard.Models.Guest
{
    public class MuseumsInformation
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "museums")]
        public List<PublicMuseum> Museums { get; set; }

        public MuseumsInformation()
        {
            Museums = new List<PublicMuseum>();
        }
    }

    public class PublicMuseum
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "openingTime")]
        public string OpeningTime { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "introduction")]
        public string Introduction { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "artifacts")]
        public List<Artifact> Artifacts { get; set; }

        public PublicMuseum()
        {
            Artifacts = new List<Artifact>();
        }
    }
}

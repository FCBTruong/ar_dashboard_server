using System;
namespace ar_dashboard.Models
{
    public class Artifact
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "image")]
        public string Image { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "modelAr")]
        public AR_Model ModelAr { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "audio")]
        public string Audio { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "information")]
        public string Information { get; set; }

        public Artifact()
        {
            Id = Guid.NewGuid().ToString();
            ModelAr = new AR_Model();
            Information = "";
        }
    }
}

using System;
using System.Collections.Generic;

namespace ar_dashboard.Models
{
    public class Museum
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "introduction")]
        public string Introduction { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "artifacts")]
        public List<Artifact> Artifacts { get; set; }

        public Museum()
        {
            Id = Guid.NewGuid().ToString();
            Name = "Default";
            Artifacts = new List<Artifact>();
        }
    }
}

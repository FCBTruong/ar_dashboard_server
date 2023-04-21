using System;
namespace ar_dashboard.Models.Guest
{
    public class ArtifactInformation
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "artifact")]
        public Artifact Artifact { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "museumId")]
        public string MuseumId { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public ArtifactInformation()
        {

        }
    }
}

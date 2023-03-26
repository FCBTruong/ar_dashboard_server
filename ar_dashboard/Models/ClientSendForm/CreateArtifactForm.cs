using System;
namespace ar_dashboard.Models.ClientSendForm
{
    public class CreateArtifactForm
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "museumId")]
        public string MuseumId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "artifact")]
        public Artifact Artifact { get; set; }
    }
}

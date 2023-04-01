using System;
namespace ar_dashboard.Models.ClientReceiveForm
{
    public class ClientReceiveArtifact
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "artifactId")]
        public string ArtifactId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "userData")]
        public UserData UserData { get; set; }
    }
}

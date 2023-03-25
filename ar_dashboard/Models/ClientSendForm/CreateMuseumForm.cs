using System;
namespace ar_dashboard.Models.ClientSendForm
{
    public class CreateMuseumForm
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}

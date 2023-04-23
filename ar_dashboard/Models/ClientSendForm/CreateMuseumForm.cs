using System;
namespace ar_dashboard.Models.ClientSendForm
{
    public class CreateMuseumForm
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "introduction")]
        public string Introduction { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "imageUrl")]
        public string ImageUrl { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
    }
}

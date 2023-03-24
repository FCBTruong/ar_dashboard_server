using System;
namespace ar_dashboard.Models.Data
{
    public class ObjectData
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}

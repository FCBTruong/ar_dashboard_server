using System;
using System.Numerics;

namespace ar_dashboard.Models
{
    public class AR_Model
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "modelAsset")]
        public Asset3D ModelAsset { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "scale")]
        public float Scale { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "position")]
        public Vector3 Position { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "rotation")]
        public Vector3 Rotation { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "information")]
        public string Information { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "status")]
        public AR_Status Status { get; set; }
    }

    public enum AR_Status : ushort
    {
        ACTIVE = 0,
        IN_ACTIVE = 1
    }
}

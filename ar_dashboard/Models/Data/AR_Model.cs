using System;
using System.Numerics;
using ar_dashboard.Models.Data;

namespace ar_dashboard.Models
{
    public class AR_Model
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "modelAsset")]
        public Asset3D ModelAsset { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "scale")]
        public SerializedVector3 Scale { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "position")]
        public SerializedVector3 Position { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "rotation")]
        public SerializedVector3 Rotation { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "information")]
        public string Information { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "status")]
        public AR_Status Status { get; set; }

        public AR_Model()
        {
            Id = Guid.NewGuid().ToString();
            Scale = new SerializedVector3(1, 1, 1);
            Position = new SerializedVector3();
            Rotation = new SerializedVector3();
            Status = AR_Status.ACTIVE;
            Information = "";
        }
    }

    public enum AR_Status : ushort
    {
        ACTIVE = 0,
        IN_ACTIVE = 1
    }
}

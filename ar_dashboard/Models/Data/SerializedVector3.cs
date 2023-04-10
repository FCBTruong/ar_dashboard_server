using System;
namespace ar_dashboard.Models.Data
{
    public class SerializedVector3
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "x")]
        public float X { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "y")]
        public float Y { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "z")]
        public float Z { get; set; }

        public SerializedVector3()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public SerializedVector3(float _X, float _Y, float _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
    }
}

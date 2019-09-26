using System.Text.Json.Serialization;

namespace Vysn.Voice.Payloads
{
    internal struct SelectProtocolPayload
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("data")]
        public SelectProtocolData Data { get; set; }
    }

    internal struct SelectProtocolData
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
    }
}
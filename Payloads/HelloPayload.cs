using System.Text.Json.Serialization;

namespace Vysn.Voice.Payloads
{
    internal struct HelloPayload
    {
        [JsonPropertyName("heartbeat_interval")]
        public long HeartbeatInterval { get; set; }
    }
}
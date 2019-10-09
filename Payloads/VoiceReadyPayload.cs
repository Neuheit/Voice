using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vysn.Voice.Payloads
{
    internal struct VoiceReadyPayload
    {
        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }

        [JsonPropertyName("ip")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("modes")]
        public ICollection<string> Modes { get; set; }

        [JsonPropertyName("heartbeat_interval")]
        public long HeartbeatInterval { get; set; }
    }
}
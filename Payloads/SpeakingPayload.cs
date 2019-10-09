using System.Text.Json.Serialization;

namespace Vysn.Voice.Payloads
{
    internal struct SpeakingPayload
    {
        [JsonPropertyName("speaking")]
        public bool IsSpeaking { get; set; }

        [JsonPropertyName("delay")]
        public long Delay { get; set; }

        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }
    }
}
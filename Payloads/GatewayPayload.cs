using System.Text.Json.Serialization;
using Vysn.Voice.Enums;

namespace Vysn.Voice.Payloads
{
    internal struct GatewayPayload<T>
    {
        [JsonPropertyName("op")]
        public VoiceOpCode Op { get; set; }

        [JsonPropertyName("d")]
        public T Data { get; set; }
    }
}
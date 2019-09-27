using System.Text.Json.Serialization;
using Vysn.Comons;

namespace Vysn.Voice.Payloads
{
    internal struct VoiceIdentifyPayload
    {
        [JsonPropertyName("server_id")]
        public Snowflake ServerId { get; set; }

        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
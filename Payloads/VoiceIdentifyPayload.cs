using System.Text.Json.Serialization;
using Vysn.Commons;
using Vysn.Commons.Converters;

namespace Vysn.Voice.Payloads
{
    internal struct VoiceIdentifyPayload
    {
        [JsonPropertyName("server_id"), JsonConverter(typeof(SnowflakeConverter))]
        public Snowflake ServerId { get; set; }

        [JsonPropertyName("user_id"), JsonConverter(typeof(SnowflakeConverter))]
        public Snowflake UserId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
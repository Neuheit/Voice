using System.Text.Json.Serialization;
using Vysn.Commons;
using Vysn.Commons.Converters;

namespace Vysn.Voice.Payloads
{
    internal struct ResumeConnectionPayload
    {
        [JsonPropertyName("server_id"), JsonConverter(typeof(SnowflakeConverter))]
        public Snowflake GuildId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
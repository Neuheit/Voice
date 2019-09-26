using System.Text.Json.Serialization;
using Vysn.Voice.Converters;

namespace Vysn.Voice.Payloads
{
    internal struct ResumeConnectionPayload
    {
        [JsonPropertyName("server_id"), JsonConverter(typeof(StringToUlongConverter))]
        public ulong ServerId { get; set; }
        
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
        
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
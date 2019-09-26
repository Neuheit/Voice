using System;
using System.Text.Json.Serialization;
using Vysn.Voice.Converters;

namespace Vysn.Voice.Responses
{
    public struct VoiceServerUpdateData
    {
        [JsonPropertyName("token")]
        public string Token { get; }

        [JsonPropertyName("guild_id"), JsonConverter(typeof(StringToUlongConverter))]
        public ulong GuildId { get; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; }

        public VoiceServerUpdateData(string token, ulong guildId, string endpoint)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            GuildId = guildId;
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }
    }
}
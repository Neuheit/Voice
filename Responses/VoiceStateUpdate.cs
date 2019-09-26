using System.Text.Json.Serialization;
using Vysn.Voice.Converters;

namespace Vysn.Voice.Responses
{
    internal struct VoiceStateUpdate
    {
        [JsonPropertyName("guild_id"), JsonConverter(typeof(StringToUlongConverter))]
        public ulong GuildId { get; set; }
        
        [JsonPropertyName("channel_id"), JsonConverter(typeof(StringToUlongConverter))]
        public ulong ChannelId { get; set; }
        
        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; set; }
        
        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; set; }
    }
}
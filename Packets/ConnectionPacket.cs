using System.Text.Json.Serialization;
using Vysn.Commons;
using Vysn.Commons.Converters;

namespace Vysn.Voice.Packets
{
    /// <summary>
    /// 
    /// </summary>
    public struct ConnectionPacket
    {
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(SnowflakeConverter))]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof(SnowflakeConverter))]
        public Snowflake UserId { get; set; }
    }
}
using Vysn.Commons;

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
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Snowflake UserId { get; set; }
    }
}
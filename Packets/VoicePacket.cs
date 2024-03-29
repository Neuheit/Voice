namespace Vysn.Voice.Packets
{
    /// <summary>
    /// </summary>
    internal struct VoicePacket
    {
        /// <summary>
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// </summary>
        public bool IsSilence { get; }

        /// <summary>
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="isSilence"></param>
        /// <param name="data"></param>
        public VoicePacket(int duration, bool isSilence, byte[] data)
        {
            Duration = duration;
            IsSilence = isSilence;
            Data = data;
        }
    }
}
namespace Vysn.Voice.Interop
{
    /// <summary>
    /// </summary>
    public readonly struct Opus
    {
        /// <summary>
        /// </summary>
        public const int SAMPLE_RATE = 48000;

        /// <summary>
        /// </summary>
        public const int CHANNELS = 2;

        /// <summary>
        /// </summary>
        public const int FRAMES = 20;

        /// <summary>
        /// </summary>
        public const int FRAME_SAMPLES = SAMPLE_RATE / 1000 * FRAMES;
    }
}
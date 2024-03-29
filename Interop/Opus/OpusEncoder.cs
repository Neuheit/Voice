using System;
using System.Runtime.InteropServices;
using Vysn.Voice.Enums;

namespace Vysn.Voice.Interop.Opus
{
    /// <summary>
    /// </summary>
    public readonly struct OpusEncoder
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

        [DllImport("opus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_create")]
        private static extern IntPtr OpusCreateEncoder(int sampleRate, int channels, int application,
            out OpusError error);

        [DllImport("opus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encode")]
        private static extern unsafe int OpusEncode(IntPtr encoder, byte* pcm, int frameSize, byte* data,
            int maxDataBytes);

        private readonly IntPtr _opusEncoder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public OpusEncoder(VoiceApplication application)
        {
            _opusEncoder = OpusCreateEncoder(SAMPLE_RATE, CHANNELS, (int) application, out var error);

            if (error != OpusError.OK)
                throw new Exception("Opus failed to create an encoder.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioData"></param>
        /// <param name="destination"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public unsafe void Encode(Span<byte> audioData, Span<byte> destination)
        {
            if (audioData.Length != destination.Length)
                throw new ArgumentOutOfRangeException(nameof(audioData), "Doesn't match destination's length.");

            int length;

            fixed (byte* audioPtr = &audioData.GetPinnableReference())
            fixed (byte* destinationPtr = &destination.GetPinnableReference())
            {
                length = OpusEncode(_opusEncoder, audioPtr, FRAME_SAMPLES, destinationPtr, destination.Length);
            }

            if (length >= 0)
                return;

            var opusError = (OpusError) length;
            throw new Exception($"Opus threw an error {opusError} when trying to encode audio data.");
        }
    }
}
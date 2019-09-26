using System;
using System.Text.Json.Serialization;
using Vysn.Voice.Enums;

namespace Vysn.Voice.Payloads
{
    internal sealed class BaseGatewayPayload
    {
        [JsonPropertyName("op")]
        public VoiceOpCode Op { get; }

        [JsonPropertyName("d")]
        public object Data { get; set; }

        public BaseGatewayPayload()
        {
            Op = Data switch
            {
                HelloPayload _              => VoiceOpCode.Hello,
                ResumeConnectionPayload _   => VoiceOpCode.Resume,
                ResumedPayload _            => VoiceOpCode.Resumed,
                SelectProtocolPayload _     => VoiceOpCode.SelectProtocol,
                SessionDescriptionPayload _ => VoiceOpCode.SessionDescription,
                SpeakingPayload _           => VoiceOpCode.Speaking,
                VoiceIdentifyPayload _      => VoiceOpCode.Identify,
                VoiceReadyPayload _         => VoiceOpCode.Ready,
                _                           => throw new ArgumentException("Failed to match data to operation type!")
            };
        }
    }
}
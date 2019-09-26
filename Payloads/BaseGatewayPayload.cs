using System;
using System.Text.Json.Serialization;
using Vysn.Voice.Entities;

namespace Vysn.Voice.Payloads
{
    internal sealed class BaseGatewayPayload
    {
        [JsonPropertyName("op")]
        public GatewayOperationType Op { get; }

        [JsonPropertyName("d")]
        public object Data { get; set; }

        public BaseGatewayPayload()
        {
            Op = Data switch
            {
                HelloPayload _              => GatewayOperationType.Hello,
                ResumeConnectionPayload _   => GatewayOperationType.Resume,
                ResumedPayload _            => GatewayOperationType.Resumed,
                SelectProtocolPayload _     => GatewayOperationType.SelectProtocol,
                SessionDescriptionPayload _ => GatewayOperationType.SessionDescription,
                SpeakingPayload _           => GatewayOperationType.Speaking,
                VoiceIdentifyPayload _      => GatewayOperationType.Identify,
                VoiceReadyPayload _         => GatewayOperationType.Ready,
                _                           => throw new ArgumentException("Failed to match data to operation type!")
            };
        }
    }
}
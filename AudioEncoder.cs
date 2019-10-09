using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Vysn.Voice.Interop;
using Vysn.Voice.Packets;

namespace Vysn.Voice
{
    internal sealed class AudioEncoder
    {
        private readonly ConcurrentQueue<VoicePacket> _packets;
        private readonly UdpClient _udpClient;

        private ReadOnlyMemory<byte> _secretKey;
        private ushort _sequence;
        public uint SSRC;
        private uint _timestamp;

        public AudioEncoder(UdpClient udpClient)
        {
            _udpClient = udpClient;
            _packets = new ConcurrentQueue<VoicePacket>();
        }

        public void SetSecret(ReadOnlyMemory<byte> secret)
            => _secretKey = secret;

        public void SetSSRC(uint ssrc)
            => SSRC = ssrc;

        public void Encode(Span<byte> audioData)
        {
            var calculatedLength = Sodium.CalculateLength(audioData);
            Span<byte> destination = stackalloc byte[calculatedLength];

            var header = destination.Slice(0, 12);

            header[0] = 0x78; //    Version + Flags
            header[1] = 0x80; //    Payload Type

            BinaryPrimitives.WriteUInt16BigEndian(header.Slice(2), _sequence);  //    WHATS THIS?
            BinaryPrimitives.WriteUInt32BigEndian(header.Slice(4), _timestamp); //    CURRENT TIMESTAMP?
            BinaryPrimitives.WriteUInt32BigEndian(header.Slice(8), SSRC);

            _sequence++;
            _timestamp += Opus.FRAME_SAMPLES;
            
            //TODO:    ENCODE WITH OPUS

            //    NONCE
            Span<byte> nonce = stackalloc byte[Sodium.NonceSize];
            Sodium.GenerateNonce(nonce.Slice(0, 4));

            //    ENCRYPT
            Sodium.Encrypt(destination.Slice(12), audioData, _secretKey.Span, nonce);
            nonce.Slice(0, 4)
                .CopyTo(destination);

            //    Queue Packet
            var packet = new VoicePacket(destination.Length, false, destination.ToArray());
            _packets.Enqueue(packet);
        }

        public async Task TransmitPacketsAsync(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                if (!_packets.TryDequeue(out var packet))
                    continue;

                await _udpClient.SendAsync(packet.Data, packet.Duration)
                    .ConfigureAwait(false);
            }
        }
    }
}
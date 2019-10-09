using System;
using System.Buffers.Binary;
using System.Net.Sockets;
using Vysn.Voice.Interop;

namespace Vysn.Voice
{
    internal sealed class AudioEncoder
    {
        private readonly UdpClient _udpClient;

        private uint _timestamp;
        private ushort _sequence;
        private uint _ssrc;
        private ReadOnlyMemory<byte> _secretKey;

        public AudioEncoder(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public void SetSecret(ReadOnlyMemory<byte> secret)
            => _secretKey = secret;

        public void SetSSRC(uint ssrc)
            => _ssrc = ssrc;

        public void BuildPacket()
        {
            //    DUMMY

            Span<byte> packet = stackalloc byte[1024];
            WriteHeader(ref packet);
            Encrypt(packet);
        }

        private void WriteHeader(ref Span<byte> packet)
        {
            packet[0] = 0x78; //    Version + Flags
            packet[1] = 0x80; //    Payload Type

            BinaryPrimitives.WriteUInt16BigEndian(packet.Slice(2), _sequence);  //    WHATS THIS?
            BinaryPrimitives.WriteUInt32BigEndian(packet.Slice(4), _timestamp); //    CURRENT TIMESTAMP?
            BinaryPrimitives.WriteUInt32BigEndian(packet.Slice(8), _ssrc);
        }

        private void Encrypt(ReadOnlySpan<byte> data)
        {
            var requiredSpace = Sodium.CalculateLength(data);

            //    NONCE STUFF
            Span<byte> nonce = stackalloc byte[Sodium.NonceSize];
            Sodium.GenerateNonce(nonce.Slice(0, 4));
        }
    }
}
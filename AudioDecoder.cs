using System;
using System.Buffers.Binary;

namespace Vysn.Voice
{
    internal sealed class AudioDecoder
    {
        public void DecryptPacket(Span<byte> audioData)
        {
            var header = audioData.Slice(0, 12);

            if (header[0] != 0x78 || header[1] != 0x80)
                throw new Exception("");

            var sequence = BinaryPrimitives.ReadUInt16BigEndian(header.Slice(2));
            var timestamp = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(4));
            var ssrc = BinaryPrimitives.ReadUInt32BigEndian(header.Slice(8));

            var encrpyted = audioData.Slice(12);
        }
    }
}
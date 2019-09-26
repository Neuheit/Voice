using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vysn.Voice.Converters
{
    public sealed class StringToUlongConverter : JsonConverter<ulong>
    {
        /// <inheritdoc />
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return 0;

            if (!Utf8Parser.TryParse(reader.ValueSpan, out ulong _, out var bytesConsumed))
                return 0;

            if (bytesConsumed != reader.ValueSpan.Length)
                return 0;

            var chars = MemoryMarshal.Cast<byte, char>(reader.ValueSpan);

            return !ulong.TryParse(chars, out var result) ? 0 : result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value}");
        }
    }
}
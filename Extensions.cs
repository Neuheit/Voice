using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Commons;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    internal static class Extensions
    {
        public static void OnDebug(this Func<LogMessage, Task> logFunc, string message)
        {
            var logMessage = new LogMessage("Vysn.Voice", LogLevel.Debug, message);
            logFunc?.Invoke(logMessage)
                .ConfigureAwait(false);
        }

        public static void OnInformation(this Func<LogMessage, Task> logFunc, string message)
        {
            var logMessage = new LogMessage("Vysn.Voice", LogLevel.Information, message);
            logFunc?.Invoke(logMessage)
                .ConfigureAwait(false);
        }

        public static void OnWarning(this Func<LogMessage, Task> logFunc, string message, Exception exception = default)
        {
            var logMessage = new LogMessage("Vysn.Voice", LogLevel.Warning, message, exception);
            logFunc?.Invoke(logMessage)
                .ConfigureAwait(false);
        }

        public static void OnException(this Func<LogMessage, Task> logFunc, string message = default,
            Exception exception = default)
        {
            var logMessage = new LogMessage("Vysn.Voice", LogLevel.Exception, message, exception);
            logFunc?.Invoke(logMessage)
                .ConfigureAwait(false);
        }

        public static Task DebugSendAsync<T>(this ClientSock clientSock, Func<LogMessage, Task> logFunc, T data)
        {
            OnDebug(logFunc, $"Payload sent -> {JsonSerializer.Serialize(data)}");
            return clientSock.SendAsync(data);
        }

        public static T DeserializePayload<T>(this ReceivedEventArgs arg)
        {
            var deserialize = JsonSerializer.Deserialize<GatewayPayload<T>>(arg.Data.Span);
            return deserialize.Data;
        }

        public static Span<byte> AsSpan(this Stream stream)
        {
            if (!stream.CanRead)
                throw new Exception("Failed to convert stream to span. Sream cannot be read.");

            Span<byte> buffer = stackalloc byte[(int) stream.Length];
            var readBytes = 0;

            while (buffer.Length != stream.Length)
            {
                var read = stream.Read(buffer.Slice(readBytes));
                if (read == 0)
                    break;

                Volatile.Write(ref readBytes, read);
            }

            // TODO: MAKE IT PRETTY
            return new Span<byte>(buffer.ToArray());
        }
    }
}
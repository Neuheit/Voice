using System;
using System.Text.Json;
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
    }
}
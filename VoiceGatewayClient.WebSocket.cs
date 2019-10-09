using System;
using System.Text.Json;
using System.Threading.Tasks;
using Socks.EventArgs;
using Vysn.Voice.Enums;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    public partial class VoiceGatewayClient
    {
        private async Task OnReceiveAsync(ReceivedEventArgs arg)
        {
            if (arg.DataSize == 0)
                return;

            OnLog?.OnDebug(arg.Raw);
            var payload = JsonSerializer.Deserialize<GatewayPayload<object>>(arg.Data.Span);

            switch (payload.Op)
            {
                case VoiceOpCode.Hello:
                    var helloPayload = arg.DeserializePayload<HelloPayload>();
                    _ = HandleHeartbeatAsync(helloPayload.HeartbeatInterval);
                    break;

                case VoiceOpCode.Ready:
                    State = ConnectionState.Ready;
                    var readyPayload = arg.DeserializePayload<VoiceReadyPayload>();
                    await StartUdpConnectionAsync(readyPayload)
                        .ConfigureAwait(false);
                    break;

                case VoiceOpCode.SessionDescription:
                    var sessionPayload = arg.DeserializePayload<SessionDescriptionPayload>();
                    if (sessionPayload.Mode != "xsalsa20_poly1305")
                    {
                        OnLog?.OnException($"{sessionPayload.Mode} mode isn't handled by Vysn.Voice.");
                        return;
                    }

                    _ = SendKeepAliveAsync()
                        .ConfigureAwait(false);
                    break;

                case VoiceOpCode.Resume:
                    break;

                case VoiceOpCode.Resumed:
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown op code: {payload.Op}");
            }
        }

        private async Task HandleHeartbeatAsync(long interval)
        {
            OnLog?.OnDebug("Started WebSocket hearbeat handling task.");

            var delay = (int) (interval * 0.75);
            while (!_connectionSource.IsCancellationRequested)
            {
                var payload = new GatewayPayload<long>
                {
                    Op = VoiceOpCode.Heartbeat,
                    Data = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                await _clientSock.DebugSendAsync(OnLog, payload)
                    .ConfigureAwait(false);

                await Task.Delay(delay, _connectionSource.Token)
                    .ConfigureAwait(false);
            }
        }
    }
}
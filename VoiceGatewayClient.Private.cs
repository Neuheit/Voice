using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Voice.Enums;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VoiceGatewayClient
    {
        private bool CanRestoreConnection(Exception exception)
            => exception switch
            {
                WebSocketException socketException
                when socketException.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely
                => true,
                TimeoutException _
                => true,
                _ when exception.InnerException != null
                => CanRestoreConnection(exception.InnerException),
                _ => false
            };

        private async Task StartUdpConnectionAsync()
        {
        }

        private async Task HandleHeartbeatAsync(int interval)
        {
            while (!_connectionSource.IsCancellationRequested)
            {
                var payload = new BaseGatewayPayload
                {
                    Data = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                await _clientSock.SendAsync(payload)
                    .ConfigureAwait(false);

                await Task.Delay(interval, _connectionSource.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task OnConnectedAsync()
        {
            State = ConnectionState.Connected;

            var payload = new BaseGatewayPayload
            {
                Data = new VoiceIdentifyPayload
                {
                    ServerId = _connectionPacket.GuildId,
                    SessionId = _connectionPacket.SessionId,
                    UserId = _connectionPacket.UserId,
                    Token = _connectionPacket.Token
                }
            };

            await _clientSock.SendAsync(payload)
                .ConfigureAwait(false);
        }

        private async Task OnDisconnectedAsync(DisconnectEventArgs arg)
        {
            State = ConnectionState.Disconnected;
            if (arg.DisconnectType == DisconnectType.Graceful)
                return;

            CanRestoreConnection(arg.Exception);
            await Task.Delay(0);
        }

        private async Task OnReceiveAsync(ReceivedEventArgs arg)
        {
            if (arg.DataSize == 0)
                return;

            var payload = JsonSerializer.Deserialize<BaseGatewayPayload>(arg.Data.Span);

            switch (payload.Op)
            {
                case VoiceOpCode.ClientDisconnect:
                    break;

                case VoiceOpCode.Heartbeat:
                    break;

                case VoiceOpCode.HeartbeatACK:
                    break;

                case VoiceOpCode.Hello:
                    var helloPayload = JsonSerializer.Deserialize<HelloPayload>(arg.Data.Span);
                    _ = HandleHeartbeatAsync((int) (helloPayload.HeartbeatInterval * 0.75));
                    break;

                case VoiceOpCode.Identify:
                    break;

                case VoiceOpCode.SelectProtocol:
                    break;

                case VoiceOpCode.Ready:
                    State = ConnectionState.Ready;
                    var readyPayload = JsonSerializer.Deserialize<VoiceReadyPayload>(arg.Data.Span);

                    break;

                case VoiceOpCode.SessionDescription:
                    break;

                case VoiceOpCode.Speaking:
                    break;

                case VoiceOpCode.Resume:
                    break;

                case VoiceOpCode.Resumed:
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown op code: {payload.Op}");
            }
        }
    }
}
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Commons;
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
                var payload = new GatewayPayload<long>
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

            var payload = new GatewayPayload<VoiceIdentifyPayload>
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

            OnLog?.Invoke(new LogMessage(nameof(Vysn.Voice), LogLevel.Debug, "Sent identify payload."));
        }

        private async Task OnDisconnectedAsync(DisconnectEventArgs arg)
        {
            OnLog?.Invoke(new LogMessage(nameof(Vysn.Voice), LogLevel.Warning,
                "Voice connection dropped."));

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

            OnLog?.Invoke(new LogMessage(nameof(Voice), LogLevel.Debug, arg.Raw));

            var payload = JsonSerializer.Deserialize<GatewayPayload<object>>(arg.Data.Span);

            switch (payload.Op)
            {
                case VoiceOpCode.ClientDisconnect:
                    break;

                case VoiceOpCode.Heartbeat:
                    break;

                case VoiceOpCode.HeartbeatACK:
                    break;

                case VoiceOpCode.Hello:
                    var helloPayload = JsonSerializer.Deserialize<GatewayPayload<HelloPayload>>(arg.Data.Span);
                    _ = HandleHeartbeatAsync((int) (helloPayload.Data.HeartbeatInterval * 0.75));
                    break;

                case VoiceOpCode.Ready:
                    State = ConnectionState.Ready;
                    var readyPayload = JsonSerializer.Deserialize<GatewayPayload<VoiceReadyPayload>>(arg.Data.Span);
                    _udpClient.Connect(readyPayload.Data.Address, readyPayload.Data.Port);

                    var selectPayload = new GatewayPayload<SelectProtocolPayload>
                    {
                        Data = new SelectProtocolPayload
                        {
                            Protocol = "udp",
                            Data = new SelectProtocolData
                            {
                                Address = readyPayload.Data.Address,
                                Port = readyPayload.Data.Port,
                                Mode = "xsalsa20_poly1305"
                            }
                        }
                    };

                    await _clientSock.SendAsync(selectPayload)
                        .ConfigureAwait(false);
                    break;

                case VoiceOpCode.SessionDescription:
                    var sessionPayload = JsonSerializer
                        .Deserialize<GatewayPayload<SessionDescriptionPayload>>(arg.Data.Span);

                    if (sessionPayload.Data.Mode != "xsalsa20_poly1305")
                    {
                        OnLog?.Invoke(new LogMessage(nameof(Vysn.Voice), LogLevel.Exception,
                            $"{sessionPayload.Data.Mode} isn't handled by Vysn."));

                        return;
                    }


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
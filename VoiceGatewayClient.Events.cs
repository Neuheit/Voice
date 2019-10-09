using System;
using System.Net.WebSockets;
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
        private async Task OnConnectedAsync()
        {
            State = ConnectionState.Connected;
            var payload = new GatewayPayload<VoiceIdentifyPayload>
            {
                Op = VoiceOpCode.Identify,
                Data = new VoiceIdentifyPayload
                {
                    ServerId = _connectionPacket.GuildId,
                    SessionId = _connectionPacket.SessionId,
                    UserId = _connectionPacket.UserId,
                    Token = _connectionPacket.Token
                }
            };

            await _clientSock.DebugSendAsync(OnLog, payload)
                .ConfigureAwait(false);
        }

        private async Task OnDisconnectedAsync(DisconnectEventArgs arg)
        {
            OnLog?.OnWarning($"Guild {_connectionPacket.GuildId} voice connection dropped. Checking restore status..");

            State = ConnectionState.Disconnected;
            if (arg.DisconnectType == DisconnectType.Graceful)
                return;

            if (CanRestoreConnection(arg.Exception))
                OnLog?.OnWarning($"Trying to restore guild {_connectionPacket.GuildId} voice connection.");
            else
                OnLog?.OnException(exception: arg.Exception);

            await Task.Delay(0);
        }

        private bool CanRestoreConnection(Exception exception)
        {
            switch (exception)
            {
                case WebSocketException socketException:
                    switch (socketException.ErrorCode)
                    {
                        case 4014:
                            OnLog?.OnException(
                                $"Guild {_connectionPacket.GuildId} deleted voice channel I was connected to.");
                            return false;

                        case 4015:
                            OnLog?.OnWarning($"Guild {_connectionPacket.GuildId} voice connection can be resumed.");
                            return true;

                        case 10054:
                            OnLog?.OnException(
                                $"Discord closed guild {_connectionPacket.GuildId} voice connection with code: 10054");
                            return false;
                    }

                    break;

                case TimeoutException timeoutException:
                    OnLog?.OnException(exception: timeoutException);
                    return true;

                case var exc when exception.InnerException != null:
                    return CanRestoreConnection(exc);

                default:
                    OnLog?.OnException(exception: exception);
                    return false;
            }

            return false;
        }
    }
}
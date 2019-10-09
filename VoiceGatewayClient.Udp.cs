using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Vysn.Voice.Enums;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    public partial class VoiceGatewayClient
    {
        private async Task StartUdpConnectionAsync(VoiceReadyPayload readyPayload)
        {
            _udpClient.Connect(readyPayload.Address, readyPayload.Port);
            var selectPayload = new GatewayPayload<SelectProtocolPayload>
            {
                Op = VoiceOpCode.SelectProtocol,
                Data = new SelectProtocolPayload
                {
                    Protocol = "udp",
                    Data = new SelectProtocolData
                    {
                        Address = readyPayload.Address,
                        Port = readyPayload.Port,
                        Mode = "xsalsa20_poly1305"
                    }
                }
            };

            await _clientSock.DebugSendAsync(OnLog, selectPayload)
                .ConfigureAwait(false);
        }

        private async Task SendKeepAliveAsync()
        {
            OnLog?.OnDebug("Started UDP keep alive task.");
            var keepAlive = (ulong) 0;

            while (!_connectionSource.IsCancellationRequested)
            {
                Volatile.Write(ref keepAlive, keepAlive + 1);
                var packet = new byte[8];
                BinaryPrimitives.WriteUInt64LittleEndian(packet, keepAlive);

                await _udpClient.SendAsync(packet, packet.Length)
                    .ConfigureAwait(false);

                await Task.Delay(5000, _connectionSource.Token)
                    .ConfigureAwait(false);
            }
        }
    }
}
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Voice.Entities;
using Vysn.Voice.Payloads;
using Vysn.Voice.Responses;

namespace Vysn.Voice
{
    public struct VoiceGatewayClient
    {
        private ClientSock _clientSock;
        private VoiceServerUpdateData _serverUpdateData;
        private readonly int _apiVersion;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiVersion"></param>
        public VoiceGatewayClient(int apiVersion)
        {
            _apiVersion = apiVersion;
            _clientSock = default;
            _serverUpdateData = default;
        }

        public async Task EstablishSocketConnectionAsync(VoiceServerUpdateData serverData)
        {
            _serverUpdateData = serverData;

            var chopped = serverData.Endpoint.AsSpan(0, serverData.Endpoint.Length - 3)
                .ToString();

            var endpoint = new Endpoint(chopped, true)
                .WithParameter("encoding", "json")
                .WithParameter("v", $"{_apiVersion}");

            _clientSock = new ClientSock(endpoint, 512);

            _clientSock.OnReceive += OnReceiveAsync;
            _clientSock.OnConnected += OnConnectedAsync;
            _clientSock.OnDisconnected += OnDisconnectedAsync;

            await _clientSock.ConnectAsync()
                .ConfigureAwait(false);
        }

        private readonly async Task OnConnectedAsync()
        {
            var payload = new BaseGatewayPayload
            {
                Data = new VoiceIdentifyPayload
                {
                }
            };

            await _clientSock.SendAsync(payload)
                .ConfigureAwait(false);
        }

        private async Task OnDisconnectedAsync(DisconnectEventArgs arg)
        {
        }

        private async Task OnReceiveAsync(ReceivedEventArgs arg)
        {
            if (arg.DataSize == 0)
                return;

            var payload = JsonSerializer.Deserialize<BaseGatewayPayload>(arg.Data.Span);

            switch (payload.Op)
            {
                case GatewayOperationType.ClientDisconnect:
                    break;

                case GatewayOperationType.Heartbeat:
                    break;

                case GatewayOperationType.HeartbeatACK:
                    break;

                case GatewayOperationType.Hello:
                    break;

                case GatewayOperationType.Identify:
                    break;
            }
        }
    }
}
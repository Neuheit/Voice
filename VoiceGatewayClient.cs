using System;
using System.Text.Json;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Voice.Enums;
using Vysn.Voice.Payloads;
using Vysn.Voice.Responses;

namespace Vysn.Voice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class VoiceGatewayClient : IAsyncDisposable
    {
        private ClientSock _clientSock;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnClientDisconnect;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnHeartbeat;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnHeartbeatACK;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnHello;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnIdentify;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnSelectProtocol;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnReady;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnSessionDescription;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnSpeaking;

        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnResume;
        
        /// <summary>
        /// 
        /// </summary>
        public event Func<object> OnResumed;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State { get; private set; }

        public async Task EstablishConnectionAsync(VoiceServerUpdateData serverData)
        {
            var chopped = serverData.Endpoint.AsSpan(0, serverData.Endpoint.Length - 3)
                .ToString();

            var endpoint = new Endpoint(chopped, true)
                .WithParameter("encoding", "json")
                .WithParameter("v", "3");

            _clientSock = new ClientSock(endpoint, 512);

            _clientSock.OnReceive += OnReceiveAsync;
            _clientSock.OnConnected += OnConnectedAsync;
            _clientSock.OnDisconnected += OnDisconnectedAsync;

            await _clientSock.ConnectAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _clientSock.DisposeAsync()
                .ConfigureAwait(false);
        }

        private async Task OnConnectedAsync()
        {
            State = ConnectionState.Connected;

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
            State = ConnectionState.Disconnected;
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
                    break;

                case VoiceOpCode.Identify:
                    break;

                case VoiceOpCode.SelectProtocol:
                    break;

                case VoiceOpCode.Ready:
                    State = ConnectionState.Ready;
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
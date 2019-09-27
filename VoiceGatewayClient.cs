using System;
using System.Text.Json;
using System.Threading.Tasks;
using Socks;
using Socks.EventArgs;
using Vysn.Comons;
using Vysn.Voice.Enums;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class VoiceGatewayClient : IAsyncDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public event Func<LogMessage, Task> OnLog;

        private ClientSock _clientSock;
        private readonly TimeSpan _connectionTimeout;
        private readonly TaskCompletionSource<bool> _readySignal;

        /// <summary>
        /// 
        /// </summary>
        public VoiceGatewayClient()
        {
            _connectionTimeout = TimeSpan.FromSeconds(30);
            _readySignal = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverData"></param>
        /// <returns></returns>
        public async Task EstablishConnectionAsync(dynamic serverData)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SendOpusAudioAsync()
        {
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
                    _readySignal.TrySetResult(true);
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
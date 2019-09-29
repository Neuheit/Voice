using System;
using System.Threading;
using System.Threading.Tasks;
using Socks;
using Vysn.Commons;
using Vysn.Voice.Enums;
using Vysn.Voice.Packets;

namespace Vysn.Voice
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VoiceGatewayClient : IAsyncDisposable
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
        private ConnectionPacket _connectionPacket;
        private readonly TimeSpan _connectionTimeout;
        private readonly CancellationTokenSource _connectionSource;

        /// <summary>
        /// 
        /// </summary>
        public VoiceGatewayClient()
        {
            _connectionTimeout = TimeSpan.FromSeconds(30);
            _connectionSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async Task RunAsync(ConnectionPacket packet)
        {
            _connectionPacket = packet;
            var chopped = packet.Endpoint.AsSpan(0, packet.Endpoint.Length - 3)
                .ToString();

            var endpoint = new Endpoint(chopped, true)
                .WithParameter("encoding", "json")
                .WithParameter("v", "3");

            _clientSock = new ClientSock(endpoint, 512);

            _clientSock.OnReceive += OnReceiveAsync;
            _clientSock.OnConnected += OnConnectedAsync;
            _clientSock.OnDisconnected += OnDisconnectedAsync;

            _ = _clientSock.ConnectAsync()
                .ConfigureAwait(false);

            var isSuccess = SpinWait.SpinUntil(() => State == ConnectionState.Connected, _connectionTimeout);

            if (!isSuccess)
            {
                await DisposeAsync();
                throw new TimeoutException("Failed to connect after waiting for 30 seconds.");
            }
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
    }
}
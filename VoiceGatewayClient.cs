using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Socks;
using Vysn.Commons;
using Vysn.Voice.Enums;
using Vysn.Voice.Packets;
using Vysn.Voice.Payloads;

namespace Vysn.Voice
{
    /// <summary>
    /// </summary>
    public partial class VoiceGatewayClient : IAsyncDisposable
    {
        /// <summary>
        /// </summary>
        public event Func<LogMessage, Task> OnLog;

        /// <summary>
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        /// </summary>
        public Snowflake GuildId { get; private set; }

        /// <summary>
        /// </summary>
        public Snowflake UserId { get; private set; }

        private readonly AudioEncoder _audioEncoder;
        private readonly CancellationTokenSource _connectionSource;

        private readonly TimeSpan _connectionTimeout;
        private readonly UdpClient _udpClient;

        private ClientSock _clientSock;
        private ConnectionPacket _connectionPacket;

        /// <summary>
        /// </summary>
        public VoiceGatewayClient(VoiceConfiguration configuration)
        {
            _connectionTimeout = TimeSpan.FromSeconds(30);
            _connectionSource = new CancellationTokenSource();
            _udpClient = new UdpClient();
            _audioEncoder = new AudioEncoder(_udpClient, configuration.Application);
            State = ConnectionState.Disconnected;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _clientSock.DisposeAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async Task RunAsync(ConnectionPacket packet)
        {
            GuildId = packet.GuildId;
            UserId = packet.UserId;
            _connectionPacket = packet;

            OnLog?.OnDebug(JsonSerializer.Serialize(packet));

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

            var isConnected = SpinWait.SpinUntil(() => State == ConnectionState.Connected, _connectionTimeout);
            if (!isConnected)
            {
                await DisposeAsync();
                OnLog?.OnException($"Guild {GuildId} connection timed out after 30 seconds.");
                throw new TimeoutException("Failed to connect after waiting for 30 seconds.");
            }

            OnLog?.OnInformation($"Guild {GuildId} voice connection established.");

            var isReady = SpinWait.SpinUntil(() => State == ConnectionState.Ready, _connectionTimeout);
            if (isReady)
            {
                _ = _audioEncoder.TransmitPacketsAsync(_connectionSource);
                OnLog?.OnDebug($"Started packet transmission for guild {GuildId}.");
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task SendAudioAsync(Stream stream)
        {
            var payload = new GatewayPayload<SpeakingPayload>
            {
                Op = VoiceOpCode.Speaking,
                Data = new SpeakingPayload
                {
                    Delay = 0,
                    IsSpeaking = true,
                    SSRC = _audioEncoder.SSRC
                }
            };

            await _clientSock.DebugSendAsync(OnLog, payload)
                .ConfigureAwait(false);

            //TODO: Convert stream to Span<byte>? IMPL BELOW:
        }
    }
}
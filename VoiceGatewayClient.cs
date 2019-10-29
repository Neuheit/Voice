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
        private readonly AudioStream _audioStream;
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
            _audioStream = new AudioStream(_audioEncoder);
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

            var chopped = packet.Endpoint.Substring(0, packet.Endpoint.Length - 3);
            var endpoint = new Endpoint(chopped, true)
                .WithParameter("encoding", "json")
                .WithParameter("v", "3");

            _clientSock = new ClientSock(endpoint, 512);

            _clientSock.OnReceive += OnReceiveAsync;
            _clientSock.OnConnected += OnConnectedAsync;
            _clientSock.OnDisconnected += OnDisconnectedAsync;

            await _clientSock.ConnectAsync()
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
                OnLog?.OnDebug($"Started packet transmission for guild {GuildId}.");
                _ = _audioEncoder.TransmitPacketsAsync(_connectionSource);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendAudioStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new InvalidOperationException("Please make sure stream can be read.");

            stream.CopyTo(_audioStream);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendNullPackets()
        {
            var frames = new[]
            {
                new byte[0xF8],
                new byte[0xFF],
                new byte[0xFE]
            };

            foreach (var frame in frames)
                _audioEncoder.Encode(frame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSpeaking"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SetSpeakingAsync(bool isSpeaking, TimeSpan delay)
        {
            if (State != ConnectionState.Connected)
                throw new InvalidOperationException($"Can't send a payload when connection state is {State}.");

            var payload = new GatewayPayload<SpeakingPayload>
            {
                Op = VoiceOpCode.Speaking,
                Data = new SpeakingPayload
                {
                    Delay = delay.Milliseconds,
                    IsSpeaking = isSpeaking,
                    SSRC = _audioEncoder.SSRC
                }
            };

            await _clientSock.DebugSendAsync(OnLog, payload)
                .ConfigureAwait(false);
        }
    }
}
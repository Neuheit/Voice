namespace Vysn.Voice.Enums
{
    internal enum VoiceOpCode
    {
        /// <summary>
        /// Begin a voice websocket connection
        /// </summary>
        Identify = 0,

        /// <summary>
        /// Select the voice protocol
        /// </summary>
        SelectProtocol = 1,

        /// <summary>
        /// Complete the websocket handshake
        /// </summary>
        Ready = 2,

        /// <summary>
        /// Keep the websocket connection alive
        /// </summary>
        Heartbeat = 3,

        /// <summary>
        /// Describe the session
        /// </summary>
        SessionDescription = 4,

        /// <summary>
        /// Indicate which users are speaking
        /// </summary>
        Speaking = 5,

        /// <summary>
        /// Resume a connection
        /// </summary>
        Resume = 7,

        /// <summary>
        /// The continuous interval in milliseconds after which the client should send a heartbeat
        /// </summary>
        Hello = 8,

        /// <summary>
        /// Acknowledge Resume
        /// </summary>
        Resumed = 9,
    }
}
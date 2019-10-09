using System;
using System.Runtime.InteropServices;

namespace Vysn.Voice.Interop
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct Sodium
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly int NonceSize
            = (int) SecretBoxNonceSize();

        /// <summary>
        /// 
        /// </summary>
        private static readonly int MacSize
            = (int) SecretBoxMacSize();

        /// <summary>
        /// 
        /// </summary>
        private static readonly int KeySize
            = (int) SecretBoxKeySize();

        [DllImport("sodium", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "crypto_secretbox_xsalsa20poly1305_noncebytes")]
        [return: MarshalAs(UnmanagedType.SysUInt)]
        private static extern UIntPtr SecretBoxNonceSize();

        [DllImport("sodium", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "crypto_secretbox_xsalsa20poly1305_macbytes")]
        [return: MarshalAs(UnmanagedType.SysUInt)]
        private static extern UIntPtr SecretBoxMacSize();

        [DllImport("sodium", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "crypto_secretbox_xsalsa20poly1305_keybytes")]
        [return: MarshalAs(UnmanagedType.SysUInt)]
        private static extern UIntPtr SecretBoxKeySize();

        [DllImport("sodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretboxEasy(ref byte cip, byte msg, ulong msgLen, byte nonce, byte key);

        [DllImport("sodium", EntryPoint = "randombytes_buf", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RandomBytesBuffer(ref byte buf, int size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="audioData"></param>
        /// <param name="key"></param>
        /// <param name="nonce"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Encrypt(Span<byte> destination, Span<byte> audioData, ReadOnlySpan<byte> key,
            Span<byte> nonce)
        {
            if (key.Length != KeySize)
                throw new ArgumentOutOfRangeException(nameof(key),
                    "Key size is not the same as Sodium key size.");

            if (nonce.Length != NonceSize)
                throw new ArgumentOutOfRangeException(nameof(nonce),
                    "Nonce size is not the same as Sodium nonce size.");

            if (audioData.Length < destination.Length + MacSize)
                throw new ArgumentOutOfRangeException(nameof(audioData),
                    "Source length is not the same as destination + MacSize.");

            var status = SecretboxEasy(ref destination.GetPinnableReference(), audioData.GetPinnableReference(),
                (ulong) audioData.Length, nonce.GetPinnableReference(), key.GetPinnableReference());

            if (status != 0)
                throw new Exception($"Sodium failed to encrypt with the following error: {status}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public static void GenerateNonce(Span<byte> buffer)
            => RandomBytesBuffer(ref buffer.GetPinnableReference(), buffer.Length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int CalculateLength(ReadOnlySpan<byte> data)
            => data.Length + MacSize;
    }
}
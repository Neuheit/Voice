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
        public static readonly int MacSize
            = (int) SecretBoxMacSize();

        /// <summary>
        /// 
        /// </summary>
        public static readonly int KeySize
            = (int) SecretBoxKeySize();

        [DllImport("sodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int SecretboxEasy(byte* buffer, byte* message, ulong messageLength, byte* nonce,
            byte* key);

        [DllImport("sodium", EntryPoint = "crypto_secretbox_open_easy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SecretboxOpenEasy(ref byte m, in byte c, int clen, in byte n, in byte k);

        [DllImport("sodium", EntryPoint = "randombytes_buf", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RandomBufferBytes(ref byte buf, int size);

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

        private static unsafe int Encrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> key,
            ReadOnlySpan<byte> nonce)
        {
            int status;

            fixed (byte* sourcePtr = &source.GetPinnableReference())
            fixed (byte* targetPtr = &target.GetPinnableReference())
            fixed (byte* keyPtr = &key.GetPinnableReference())
            fixed (byte* noncePtr = &nonce.GetPinnableReference())
                status = SecretboxEasy(targetPtr, sourcePtr, (ulong) source.Length, noncePtr, keyPtr);

            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public static void GenerateNonce(Span<byte> buffer)
            => RandomBufferBytes(ref buffer.GetPinnableReference(), buffer.Length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int CalculateLength(ReadOnlySpan<byte> data)
            => data.Length + MacSize;
    }
}
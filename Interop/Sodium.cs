using System;
using System.Runtime.InteropServices;

namespace Vysn.Voice.Interop
{
    /// <summary>
    /// 
    /// </summary>
    public struct Sodium
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
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="nonce"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool TryEncrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> nonce,
            ReadOnlyMemory<byte> key)
        {
            if (nonce.Length != NonceSize)
                throw new Exception($"Source nonce length ({nonce.Length}) didn't match {NonceSize} size.");

            var targetExpected = MacSize + source.Length;
            if (target.Length != targetExpected)
                throw new Exception(
                    $"Target length ({target.Length}) didn't match expected ({targetExpected}) length.");

            if (key.Length != KeySize)
                throw new Exception($"Input key's length ({key.Length}) doesn't match KeySize ({KeySize}) length.'");

            int result;
            if ((result = Encrypt(source, target, key.Span, nonce)) == 0)
                return true;

            throw new Exception($"Failed to encrypt buffer -> {result}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public static void GenerateRandomBytes(Span<byte> buffer)
            => RandomBufferBytes(ref buffer.GetPinnableReference(), buffer.Length);
    }
}
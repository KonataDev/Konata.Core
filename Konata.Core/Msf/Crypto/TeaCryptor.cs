using System;
using Konata.Library.TeaEncrypt;

namespace Konata.Msf.Crypto
{
    public sealed class TeaCryptor : ICryptor
    {
        public static TeaCryptor Instance { get; } = new TeaCryptor();

        private static Tea _tea = new Tea();

        private TeaCryptor() { }

        /// <summary>
        /// Encrypt data.
        /// </summary>
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            return _tea.Encrypt(data, key);
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            return _tea.Decrypt(data, key);
        }

        /// <summary>
        /// Encrypt data with default key.
        /// </summary>
        public byte[] Encrypt(byte[] data) => Encrypt(data, new byte[16]);

        /// <summary>
        /// Decrypt data with default key.
        /// </summary>
        public byte[] Decrypt(byte[] data) => Decrypt(data, new byte[16]);
    }
}
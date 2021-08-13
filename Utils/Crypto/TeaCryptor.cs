using System;
using System.Collections.Generic;
using System.Text;
using Konata.Core.Utils.TeaEncrypt;

namespace Konata.Core.Utils.Crypto
{
    public sealed class TeaCryptor : ICryptor
    {
        public static TeaCryptor Instance { get; } = new TeaCryptor();

        private TeaCryptor() { }

        /// <summary>
        /// Encrypt data.
        /// </summary>
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            return Tea.Encrypt(data, key);
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            return Tea.Decrypt(data, key);
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

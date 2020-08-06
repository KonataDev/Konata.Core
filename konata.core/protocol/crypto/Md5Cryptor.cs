using System;
using System.Security.Cryptography;

namespace Konata.Protocol.Crypto
{
    public class Md5Cryptor : IKonataCryptor
    {

        public byte[] Decrypt(byte[] data) => null;

        public byte[] Decrypt(byte[] data, byte[] key) => null;

        public byte[] Encrypt(byte[] data)
        {
            return MD5.Create().ComputeHash(data);
        }

        public byte[] Encrypt(byte[] data, byte[] key) => null;
    }
}

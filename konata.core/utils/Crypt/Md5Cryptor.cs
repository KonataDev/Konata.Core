using System;
using System.Security.Cryptography;
using Konata.Utils.Crypt;

namespace Konata.Utils.Crypt
{
    public class Md5Cryptor : ICryptor
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

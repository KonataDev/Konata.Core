using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Konata.Core.Utils.Crypto
{
    public class Md5Cryptor : ICryptor
    {

        public byte[] Decrypt(byte[] data) => null;

        public byte[] Decrypt(byte[] data, byte[] key) => null;

        public byte[] Encrypt(byte[] data)
        {
            return MD5.Create().ComputeHash(data);
        }

        public byte[] Encrypt(byte[] data, byte[] key)
        {
            return MD5.Create().ComputeHash(data);
        }
    }
}

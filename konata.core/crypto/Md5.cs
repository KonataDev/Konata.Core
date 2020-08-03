using System;
using System.Security.Cryptography;

namespace Konata.Crypto
{
    public static class Md5
    {
        public static byte[] Create(byte[] data)
        {
            return HashAlgorithm.Create("MD5").ComputeHash(data);
        }
    }
}

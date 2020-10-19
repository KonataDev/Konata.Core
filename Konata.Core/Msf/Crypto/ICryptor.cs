using System;

namespace Konata.Msf.Crypto
{
    public interface ICryptor
    {
        byte[] Encrypt(byte[] data);

        byte[] Encrypt(byte[] data, byte[] key);

        byte[] Decrypt(byte[] data);

        byte[] Decrypt(byte[] data, byte[] key);

    }
}

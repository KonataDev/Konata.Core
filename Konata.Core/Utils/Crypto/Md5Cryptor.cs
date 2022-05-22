using System.Security.Cryptography;

namespace Konata.Core.Utils.Crypto;

internal class Md5Cryptor : ICryptor
{
    public byte[] Encrypt(byte[] data)
    {
#if NET5_0_OR_GREATER
        return MD5.HashData(data);
#else
        using MD5 md5 = MD5.Create();
        return md5.ComputeHash(data);
#endif
    }

    public byte[] Encrypt(byte[] data, byte[] key)
        => Encrypt(data);
}

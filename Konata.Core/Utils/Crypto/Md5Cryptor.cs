using System.Security.Cryptography;

namespace Konata.Core.Utils.Crypto;

internal class Md5Cryptor : ICryptor
{
    public byte[] Encrypt(byte[] data)
        => MD5.Create().ComputeHash(data);

    public byte[] Encrypt(byte[] data, byte[] key)
        => MD5.Create().ComputeHash(data);
}

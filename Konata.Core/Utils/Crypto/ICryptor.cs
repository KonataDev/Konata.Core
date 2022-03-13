using System;

namespace Konata.Core.Utils.Crypto;

internal interface ICryptor
{
    virtual byte[] Encrypt(byte[] data)
        => throw new NotSupportedException();

    virtual byte[] Encrypt(byte[] data, byte[] key)
        => throw new NotSupportedException();

    virtual byte[] Decrypt(byte[] data)
        => throw new NotSupportedException();

    virtual byte[] Decrypt(byte[] data, byte[] key)
        => throw new NotSupportedException();
}

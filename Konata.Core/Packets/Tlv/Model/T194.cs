﻿using System.Text;
using Konata.Core.Utils.Crypto;

namespace Konata.Core.Packets.Tlv.Model;

internal class T194Body : TlvBody
{
    public readonly byte[] _imsiMd5;

    public T194Body(string imsi)
        : base()
    {
        _imsiMd5 = new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(imsi));

        PutBytes(_imsiMd5);
    }

    public T194Body(byte[] data)
        : base(data)
    {
        TakeBytes(out _imsiMd5, Prefix.None);
    }
}

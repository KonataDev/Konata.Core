﻿namespace Konata.Core.Packets.Tlv.Model;

// 與T197相同?

internal class T198Body : TlvBody
{
    public readonly byte _devLockMobileType;

    public T198Body(byte devLockMobileType)
        : base()
    {
        _devLockMobileType = devLockMobileType;

        PutByte(_devLockMobileType);
    }

    public T198Body(byte[] data)
        : base(data)
    {
        TakeByte(out _devLockMobileType);
    }
}

﻿namespace Konata.Core.Packets.Tlv.Model;

internal class T17aBody : TlvBody
{
    public readonly uint _smsAppId;

    public T17aBody(uint smsAppId)
        : base()
    {
        _smsAppId = smsAppId;

        PutUintBE(_smsAppId);
    }

    public T17aBody(byte[] data)
        : base(data)
    {
        TakeUintBE(out _smsAppId);
    }
}

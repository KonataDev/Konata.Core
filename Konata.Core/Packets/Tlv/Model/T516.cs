﻿namespace Konata.Core.Packets.Tlv.Model;

internal class T516Body : TlvBody
{
    public readonly uint _sourceType;

    public T516Body(uint sourceType = 0)
        : base()
    {
        _sourceType = sourceType;

        PutUintBE(_sourceType);
    }

    public T516Body(byte[] data)
        : base(data)
    {
        TakeUintBE(out _sourceType);
    }
}

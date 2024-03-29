﻿namespace Konata.Core.Packets.Tlv.Model;

internal class T104Body : TlvBody
{
    public readonly string _sigSession;

    public T104Body(string sigSession)
        : base()
    {
        PutString(sigSession);
    }

    public T104Body(byte[] data)
        : base(data)
    {
        TakeString(out _sigSession, Prefix.None);
    }
}

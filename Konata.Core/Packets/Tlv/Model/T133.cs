﻿namespace Konata.Core.Packets.Tlv.Model;

internal class T133Body : TlvBody
{
    public readonly byte[] _wtSessionTicketSig;

    public T133Body(byte[] wtSessionTicketSig, object nil)
        : base()
    {
        _wtSessionTicketSig = wtSessionTicketSig;

        PutBytes(_wtSessionTicketSig);
    }

    public T133Body(byte[] data)
        : base(data)
    {
        TakeBytes(out _wtSessionTicketSig, Prefix.None);
    }
}

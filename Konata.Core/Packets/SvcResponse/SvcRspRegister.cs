﻿using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Packets.SvcResponse;

internal class SvcRspRegister : UniPacket
{
    public long uin;
    public long bid;
    public string ipAddress;
    public bool status;

    public SvcRspRegister(byte[] response)
        : base(response, (userdata, r) =>
        {
            var p = (SvcRspRegister) userdata;
            {
                p.uin = (JNumber) r["0.0"];
                p.bid = (JNumber) r["0.1"];
                p.status = (JNumber) r["0.9"] == 1;
                p.ipAddress = (string) (JString) r["0.10"];
            }
        })
    {
    }
}

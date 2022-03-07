using System;
using System.Collections.Generic;
using Konata.Core.Common;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcResponse;

internal class SvcRspHttpServerListRsp : UniPacket
{
    public List<ServerInfo> Servers { get; private set; }

    public SvcRspHttpServerListRsp(byte[] payload)
        : base(payload, (userdata, r) =>
        {
            // Initialize
            var p = (SvcRspHttpServerListRsp) userdata;
            p.Servers = new();

            // Get servers
            foreach (JStruct info in r["0.2"].List)
            {
                p.Servers.Add(new(
                    info[1].String.Value,
                    (ushort) info[2].Number.ValueInt
                ));
            }
        })
    {
    }
}

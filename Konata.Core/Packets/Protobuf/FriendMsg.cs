using System;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

internal class FriendMsg : ProtoTreeRoot
{
    public FriendMsg(uint friendUin, (uint, uint) consts, byte[] chain)
    {
        AddTree("0A", (root) =>
        {
            // Add source node
            root.AddTree("0A", _ => _.AddLeafVar("08", friendUin));
        });

        // Add slice node
        AddTree("12", (slice) =>
        {
            slice.AddLeafVar("08", 1);
            slice.AddLeafVar("10", 0);
            slice.AddLeafVar("18", 0);
        });

        // Add message content node
        AddTree("1A", message => message.AddLeafBytes("0A", chain));

        // Add random request id
        AddLeafVar("20", new Random().Next(1000, 65536));

        // Add ramdom bytes
        AddLeafVar("28", new Random().Next(int.MaxValue));

        // Unknown flag
        AddTree("32", new SyncCookie(consts));

        // Unknown flag
        AddLeafVar("40", 0);
    }
}

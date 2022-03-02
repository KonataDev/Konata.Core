using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway.Requests;

internal class MultiMsgUpRequest : ProtoTreeRoot
{
    public MultiMsgUpRequest(uint destUin, byte[] msgPacked, byte[] ticket)
    {
        AddLeafVar("08", 1);
        AddLeafVar("10", 5);
        AddLeafVar("18", 9);

        AddTree("22", _ =>
        {
            _.AddLeafVar("08", 1);
            _.AddLeafVar("10", destUin);
            _.AddLeafBytes("22", msgPacked);
            _.AddLeafVar("28", 2);
            _.AddLeafBytes("32", ticket);
        });

        AddLeafVar("50", 1);
    }
}

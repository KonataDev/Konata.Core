using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

internal class GroupMsgReadReport : ProtoTreeRoot
{
    public GroupMsgReadReport(uint groupUin, uint requestId)
    {
        AddTree("0A", (root) =>
        {
            root.AddLeafVar("08", groupUin);
            root.AddLeafVar("10", requestId);
        });
    }
}

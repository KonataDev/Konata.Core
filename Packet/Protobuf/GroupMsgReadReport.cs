using System;
using System.Text;

using Konata.Utils.Protobuf;

namespace Konata.Core.Packet.Protobuf
{
    public class GroupMsgReadedReport : ProtoTreeRoot
    {
        public GroupMsgReadedReport(uint groupUin, uint requestId)
        {
            AddTree("0A", (root) =>
            {
                root.AddLeafVar("08", groupUin);
                root.AddLeafVar("10", requestId);
            });
        }
    }
}

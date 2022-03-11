using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

/// <summary>
/// 撤回群訊息 PbMsgWithDraw
/// </summary>
internal class ProtoMsgWithDraw : ProtoTreeRoot
{
    public ProtoMsgWithDraw(uint group, uint msgId) : base()
    {
        AddTree("12", (t) =>
        {
            t.AddLeafVar("08", 1);
            t.AddLeafVar("10", 0);
            t.AddLeafVar("18", group);

            t.AddTree("22", (t2) =>
            {
                t2.AddLeafVar("08", 7499);
                t2.AddLeafVar("10", msgId);
            });

            t.AddTree("2A", (t2) => t2.AddLeafVar("08", 0));
        });
    }
}

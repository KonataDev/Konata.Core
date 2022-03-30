using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

/// <summary>
/// 撤回群訊息 PbMsgWithDraw
/// </summary>
internal class ProtoMsgWithDraw : ProtoTreeRoot
{
    public ProtoMsgWithDraw(uint groupUin, uint msgSeq, uint msgRand)
    {
        AddTree("12", (t) =>
        {
            t.AddLeafVar("08", 1);
            t.AddLeafVar("10", 0);
            t.AddLeafVar("18", groupUin);

            t.AddTree("22", (t2) =>
            {
                t2.AddLeafVar("08", msgSeq);
                t2.AddLeafVar("10", msgRand);
            });

            t.AddTree("2A", (t2) => t2.AddLeafVar("08", 0));
        });
    }

    public ProtoMsgWithDraw(uint selfUin, uint friendUin, uint msgSeq, uint msgRand, long uuid, uint time)
    {
        AddTree("0A", (t) =>
        {
            t.AddTree("0A", _ =>
            {
                _.AddLeafVar("08", selfUin);
                _.AddLeafVar("10", friendUin);
                _.AddLeafVar("18", msgSeq);
                _.AddLeafVar("20", uuid);
                _.AddLeafVar("28", time);
                _.AddLeafVar("30", msgRand);
            });
            t.AddLeafVar("10", 0);
            t.AddTree("1A", _ => _.AddLeafVar("08", 0));
            t.AddLeafVar("20", 1);
        });
    }
}

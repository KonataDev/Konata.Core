using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

internal class SharpVideoMsg : ProtoTreeRoot
{
    public SharpVideoMsg(uint selfUin, uint friendUin)
    {
        AddTree("0A", _ =>
        {
            _.AddLeafVar("08", 0);
            _.AddLeafVar("10", selfUin);
            _.AddLeafVar("18", 0); // sequence
            _.AddLeafVar("20", 1);
            _.AddLeafString("8A01", ",-1");
        });

        AddTree("12", _ =>
        {
            _.AddLeafVar("08", 1);
            _.AddLeafVar("10", friendUin);
            _.AddLeafVar("18", 1);
            _.AddLeafFix32("25", 0); // fix32 ?
            _.AddLeafVar("28", 0);
            _.AddTree("3A", __ =>
            {
                __.AddLeafVar("08", friendUin);
                __.AddLeafVar("10", 1);
                __.AddLeafVar("18", 0);
                __.AddLeafVar("28", 0);
                __.AddLeafVar("40", 0);
                __.AddLeafVar("48", 0);
            });
            _.AddLeafVar("40", 0);
        });
    }

    internal class Ack : ProtoTreeRoot
    {
        public Ack(long roomId, uint selfUin)
        {
            AddTree("0A", _ =>
            {
                _.AddLeafVar("08", roomId);
                _.AddLeafVar("10", selfUin);
                _.AddLeafVar("18", 0); // seq ?
                _.AddLeafVar("20", 4);
            });

            AddTree("1A", _ =>
            {
                _.AddLeafVar("08", 4);
                _.AddLeafVar("10", 109);
                _.AddLeafVar("18", 0);
            });
        }
    }
}

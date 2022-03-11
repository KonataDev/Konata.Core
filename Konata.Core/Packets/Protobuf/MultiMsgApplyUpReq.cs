using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

public class MultiMsgApplyUpReq : ProtoTreeRoot
{
    public MultiMsgApplyUpReq(uint destUin, uint msgLen, byte[] msgMd5)
    {
        AddLeafVar("08", 1);
        AddLeafVar("10", 5);
        AddLeafVar("18", 9);
        AddLeafVar("20", 3);
        AddLeafString("2A", AppInfo.AppBuildVer);

        AddTree("32", _ =>
        {
            _.AddLeafVar("08", destUin);
            _.AddLeafVar("10", msgLen);
            _.AddLeafBytes("1A", msgMd5);
            _.AddLeafVar("20", 1);
            _.AddLeafVar("28", 0);
        });

        AddLeafVar("40", 2);
    }
}

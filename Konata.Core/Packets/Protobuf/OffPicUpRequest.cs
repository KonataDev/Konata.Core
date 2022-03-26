using System.Collections.Generic;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf;

internal class OffPicUpRequest : ProtoTreeRoot
{
    /// <summary>
    /// Single picture information
    /// </summary>
    private class PicInfo : ProtoTreeRoot
    {
        public PicInfo(uint friendUin, uint selfUin, ImageChain chain)
        {
            AddLeafVar("08", selfUin);
            AddLeafVar("10", friendUin);
            AddLeafVar("18", 0);

            // MD5 Byte
            AddLeafBytes("22", chain.HashData);

            // Image length
            AddLeafVar("28", chain.FileLength);

            // Image file name
            AddLeafString("32", chain.FileHash);

            // Unknown
            AddLeafVar("38", 5);
            AddLeafVar("40", 9);
            AddLeafVar("50", 0);
            AddLeafVar("60", 1);
            AddLeafVar("68", 0);

            // Image Width
            AddLeafVar("70", chain.Width);

            // Image Height
            AddLeafVar("78", chain.Height);

            // Image type
            AddLeafVar("8001", (long) chain.ImageType);

            // Version
            AddLeafString("8A01", AppInfo.AppBuildVer);

            // Original image
            AddLeafVar("A801", 0);
            AddLeafVar("B001", 0);
        }
    }

    public OffPicUpRequest(uint groupUin,
        uint selfUin, List<ImageChain> chains)
    {
        AddLeafVar("08", 1);
        AddLeafVar("50", 3);

        foreach (var i in chains)
        {
            AddTree("12", new PicInfo(groupUin, selfUin, i));
        }
    }
}

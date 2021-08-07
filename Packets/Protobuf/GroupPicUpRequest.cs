using System;
using System.Collections.Generic;

using Konata.Utils.Protobuf;
using Konata.Core.Message.Model;

namespace Konata.Core.Packets.Protobuf
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupPicUpRequest : ProtoTreeRoot
    {
        /// <summary>
        /// Single picture information
        /// </summary>
        private class PicInfo : ProtoTreeRoot
        {
            public PicInfo(uint groupUin, uint memberUin, ImageChain chain)
                : base()
            {
                AddLeafVar("08", groupUin);
                AddLeafVar("10", memberUin);
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
                AddLeafVar("48", 1);

                // Image Width
                AddLeafVar("50", chain.Width);

                // Image Height
                AddLeafVar("58", chain.Height);

                // Image type
                AddLeafVar("60", (long)chain.ImageType);

                // Version
                AddLeafString("6A", AppInfo.AppBuildVer);
                AddLeafVar("78", 1052);

                // Original image
                AddLeafVar("8001", 0);
                AddLeafVar("9801", 0);
            }
        }

        public GroupPicUpRequest(uint groupUin,
            uint memberUin, List<ImageChain> chains) : base()
        {
            AddLeafVar("08", 3);
            AddLeafVar("10", 1);

            foreach (var i in chains)
            {
                AddTree("1A", new PicInfo(groupUin, memberUin, i));
            }
        }
    }
}

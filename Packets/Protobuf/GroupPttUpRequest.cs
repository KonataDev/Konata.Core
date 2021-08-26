using Konata.Core.Message.Model;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf
{
    public class GroupPttUpRequest : ProtoTreeRoot
    {
        /// <summary>
        /// Single record information
        /// </summary>
        private class RecordInfo : ProtoTreeRoot
        {
            public RecordInfo(uint groupUin, uint selfUin, RecordChain chain)
            {
                AddLeafVar("08", groupUin);
                AddLeafVar("10", selfUin);
                AddLeafVar("18", 0);

                // MD5 Byte
                AddLeafBytes("22", chain.HashData);

                // Record length
                AddLeafVar("28", chain.FileLength);

                // Record file name
                AddLeafString("32", chain.FileHash);

                // Unknown
                AddLeafVar("38", 5);
                AddLeafVar("40", 9);
                AddLeafVar("48", 3);
                AddLeafString("52", AppInfo.AppBuildVer);

                // Record time seconds
                AddLeafVar("60", chain.TimeSeconds);

                // TODO:
                // Maybe record type
                // inside it
                AddLeafVar("68", 1);
                AddLeafVar("70", 1);
                AddLeafVar("78", 2);
            }
        }

        public GroupPttUpRequest(uint groupUin,
            uint selfUin, RecordChain chain)
        {
            AddLeafVar("08", 3);
            AddLeafVar("10", 3);
            AddTree("2A", new RecordInfo(groupUin, selfUin, chain));
        }
    }
}

using Konata.Core.Common;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway.Requests;

internal class GroupPttUpRequest : ProtoTreeRoot
{
    /// <summary>
    /// Single record information
    /// </summary>
    private class RecordInfo : ProtoTreeRoot
    {
        public RecordInfo(AppInfo appInfo, uint groupUin, uint selfUin, RecordChain chain)
        {
            AddLeafVar("08", groupUin);
            AddLeafVar("10", selfUin);
            AddLeafVar("18", 0);

            // MD5 Byte
            AddLeafBytes("22", chain.HashData);

            // Record length
            AddLeafVar("28", chain.FileLength);

            // Record file name
            AddLeafString("32", chain.FileName);

            // Unknown
            AddLeafVar("38", 5);
            AddLeafVar("40", 9);
            AddLeafVar("48", 3);
            AddLeafString("52", appInfo.AppBuildVer);

            // Record time seconds
            AddLeafVar("60", chain.TimeSeconds);

            AddLeafVar("68", 1);
            AddLeafVar("70", (int) chain.RecordType);
            AddLeafVar("78", 1);
        }
    }

    public GroupPttUpRequest(AppInfo appInfo, uint groupUin,
        uint selfUin, RecordChain chain)
    {
        AddLeafVar("08", 3);
        AddLeafVar("10", 3);
        AddTree("2A", new RecordInfo(appInfo, groupUin, selfUin, chain));
    }
}

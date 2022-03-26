using Konata.Core.Message.Model;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway.Requests;

internal class FriendPttUpRequest : ProtoTreeRoot
{
    /// <summary>
    /// Single record information
    /// </summary>
    private class RecordInfo : ProtoTreeRoot
    {
        public RecordInfo(uint friendUin, uint selfUin, RecordChain chain)
        {
            AddLeafVar("08", 500); // command
            AddLeafVar("10", 0); // 0

            AddTree("3A", _ =>
            {
                _.AddLeafVar("50", selfUin); // sender
                _.AddLeafVar("A001", friendUin); //recver
                _.AddLeafVar("F001", 2); // file type record (2) 
                _.AddLeafVar("C002", chain.FileLength); // file size
                _.AddLeafString("9203", chain.FileName); // file name
                _.AddLeafBytes("E203", chain.HashData); // file md5
            });

            // Business id
            AddLeafVar("A806", 17);

            // Client type
            AddLeafVar("B006", 104);

            // Extension req
            AddTree("FAE930", _ =>
            {
                _.AddLeafVar("08", 3); // id
                _.AddLeafVar("10", 0); // type
                _.AddLeafVar("E08B2C", (long) chain.RecordType); // ptt type
                _.AddLeafVar("A0982C", 3); // net type
                _.AddLeafVar("C09E2C", (long) chain.RecordType); // voice type
                _.AddLeafVar("80AB2C", chain.TimeSeconds); // ptt time
            });
        }
    }

    public FriendPttUpRequest(uint friendUin,
        uint selfUin, RecordChain chain)
    {
        AddTree(new RecordInfo(friendUin, selfUin, chain));
    }
}

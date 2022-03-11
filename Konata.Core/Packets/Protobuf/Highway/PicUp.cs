using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway;

internal class PicUp : ProtoTreeRoot
{
    public enum CommandId
    {
        // Friend picture / record
        FriendPicDataUp = 1,
        FriendPttDataUp = 26,

        // Group picture / record / file
        GroupPicDataUp = 2,
        GroupPttDataUp = 29,
        GroupFileDataUp = 71,

        // Avatar
        SelfPortrait = 5,

        // Multl message
        MultiMsgDataUp = 27
    }

    public PicUp(string cmd, CommandId cmdid, uint peerUin, int sequence)
    {
        AddTree("0A", (w) =>
        {
            // Version
            w.AddLeafVar("08", 1);

            // Uin string
            w.AddLeafString("12", peerUin.ToString());

            // Command
            w.AddLeafString("1A", cmd);

            // Sequence
            w.AddLeafVar("20", sequence);

            // Retry times
            w.AddLeafVar("28", 0);

            // App id
            w.AddLeafVar("30", AppInfo.SubAppId);

            // Normal Flag
            w.AddLeafVar("38", 4096);

            // Command id
            w.AddLeafVar("40", (long) cmdid);

            // Locale id
            w.AddLeafVar("50", 2052);
        });
    }
}

using System;

using Konata.Core.Packets.Wup;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcPush
{
    public class SvcReqPushMsg : UniPacket
    {
        //public uint FromUin;
        //public uint FromGroup;
        //public uint OperatorUin;
        //public uint AffectedUin;
        //public uint ReceiveTime;

        public PushType EventType;

        public byte[] PushPayload;

        public SvcReqPushMsg(byte[] pushMsg)
            : base(pushMsg, (userdata, r) =>
            {
                var p = (SvcReqPushMsg)userdata;

                var list = (JList)r["0.2"];
                {
                    // Parse type and data
                    p.EventType = (PushType)(int)(JNumber)list["0.2"];
                    p.PushPayload = (byte[])(JSimpleList)list["0.6"];
                }
            })
        {

        }
    }

    public enum PushType : int
    {
        Friend = 528,
        Group = 732
    }

    public enum PushSubType : int
    {
        GroupRecallMessage,
        GroupMute,
        GroupNewMember,
        GroupSettingsUpload,
        GroupSettingsFranklySpeeking,
        GroupSettingsDirectMessage,
        GroupSettingsAnonymous,
        GroupSettingsGroupComposition,

        //FriendRecallMessage,
    }
}

using System;
using Konata.Library.Protobuf;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Msf.Services.OnlinePush
{
    public class PbPushGroupMsg : Service
    {
        private PbPushGroupMsg()
        {
            Register("OnlinePush.PbPushGroupMsg", this);
        }

        public static Service Instance { get; } = new PbPushGroupMsg();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            if (args == null)
                return false;
            if (args.Length != 1)
                return false;
            if (args[0] is byte[] payload)
                return Handle_PbPushGroupMsg(core, new ProtoPbPushGroupMsg(payload));

            return false;
        }

        private bool Handle_PbPushGroupMsg(Core core, ProtoPbPushGroupMsg msg)
        {
            core.PostUserEvent(EventType.GroupMessage,
                msg.GroupUin, msg.MemberUin, msg.MsgContent);
            return true;
        }
    }
}

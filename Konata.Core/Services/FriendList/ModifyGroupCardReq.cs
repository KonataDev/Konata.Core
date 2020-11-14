using System;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.SvcRequest;

namespace Konata.Services.FriendList
{
    public class ModifyGroupCardReq : Service
    {
        private ModifyGroupCardReq()
        {
            Register("friendlist.ModifyGroupCardReq", this);
        }

        public static Service Instance { get; } = new ModifyGroupCardReq();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 3)
                return false;

            if (args[0] is uint groupUin
                && args[1] is uint memberUin
                && args[2] is string memberCard)
                return Request_ModifyGroupCardReq(core, groupUin, memberUin, memberCard);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        public bool Request_ModifyGroupCardReq(Core core, uint groupUin,
            uint memberUin, string memberCard)
        {
            var ssoSeq = core.SsoMan.GetNewSequence();
            var ssoSession = core.SsoMan.GetSsoSession();

            var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
                new SvcReqModifyGroupCard(groupUin, memberUin, memberCard));

            return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
                ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
        }
    }
}

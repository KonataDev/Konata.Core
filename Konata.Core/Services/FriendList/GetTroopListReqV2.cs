using System;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.SvcRequest;

namespace Konata.Services.FriendList
{
    public class GetTroopListReqV2 : Service
    {
        private GetTroopListReqV2()
        {
            Register("friendlist.GetTroopListReqV2", this);
        }

        public static Service Instance { get; } = new GetTroopListReqV2();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 1)
                return false;

            if (args[0] is uint selfUin)
                return Request_GetTroopListReqV2(core, selfUin);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        public bool Request_GetTroopListReqV2(Core core, uint selfUin)
        {
            var ssoSeq = core.SsoMan.GetNewSequence();
            var ssoSession = core.SsoMan.GetSsoSession();

            var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
                new SvcReqGetTroopListReqV2Simplify(selfUin));

            return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
                ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
        }
    }
}

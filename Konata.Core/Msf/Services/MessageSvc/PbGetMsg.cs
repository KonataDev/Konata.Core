using System;
using Konata.Msf.Packets;
using Konata.Msf.Packets.Sso;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Msf.Services.MessageSvc
{
    public class PbGetMsg : Service
    {
        private PbGetMsg()
        {
            Register("MessageSvc.PbGetMsg", this);
        }

        public static Service Instance { get; } = new PbGetMsg();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_PbGetMsg(core);
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }

        private bool Request_PbGetMsg(Core core)
        {
            var ssoSeq = core.SsoMan.GetNewSequence();
            var ssoSession = core.SsoMan.GetSsoSession();

            var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
                new ProtoGetMsg(core.SigInfo.SyncCookie).Serialize());

            return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
                ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
        }

        private bool Handle_PbGetMsg(Core core)
        {
            return true;
        }
    }
}

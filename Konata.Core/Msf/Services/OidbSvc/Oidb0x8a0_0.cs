using System;
using Konata.Msf.Packets;
using Konata.Msf.Packets.Sso;
using Konata.Msf.Packets.Oidb;

namespace Konata.Msf.Services.OidbSvc
{
    public class Oidb0x8a0_0 : Service
    {
        private Oidb0x8a0_0()
        {
            Register("OidbSvc.0x8a0_0", this);
        }

        public static Service Instance { get; } = new Oidb0x8a0_0();

        public override bool OnRun(Core core, string method,
            params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 3)
                return false;

            if (args[0] is uint groupUin
                && args[1] is uint[] memberUin
                && args[2] is bool preventRequest)
                return Request_0x8a0_0(core, groupUin, memberUin, preventRequest);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        private bool Request_0x8a0_0(Core core, uint groupUin,
            uint[] memberUin, bool preventRequest)
        {
            var ssoSeq = core.SsoMan.GetNewSequence();
            var ssoSession = core.SsoMan.GetSsoSession();

            var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
                new OidbCmd0x8a0_0(groupUin, memberUin, preventRequest));

            return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
                ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
        }
    }
}

using System;
using Konata.Msf.Packets;
using Konata.Msf.Packets.Sso;
using Konata.Msf.Packets.Oidb;

namespace Konata.Msf.Services.OidbSvc
{
    public class Oidb0x8fc_2 : Service
    {
        private Oidb0x8fc_2()
        {
            Register("OidbSvc.0x8fc_2", this);
        }

        public static Service Instance { get; } = new Oidb0x8fc_2();


        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 3
                || args.Length != 4)
                return false;

            if (args[0] is uint groupUin
                && args[1] is uint memberUin
                && args[2] is string specialTitle)
                return Request_0x8fc_2(core, groupUin, memberUin,
                    specialTitle, args.Length == 4 ? ((uint?)args[3]) : null);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        private bool Request_0x8fc_2(Core core, uint groupUin, uint memberUin,
            string specialTitle, uint? expiredTime)
        {
            var ssoSeq = core.SsoMan.GetNewSequence();
            var ssoSession = core.SsoMan.GetSsoSession();

            var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
                new OidbCmd0x8fc_2(groupUin, memberUin,
                specialTitle, expiredTime));

            return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
                ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
        }
    }
}

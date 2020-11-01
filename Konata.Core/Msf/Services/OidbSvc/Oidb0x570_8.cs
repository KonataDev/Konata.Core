using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Msf.Services.OidbSvc
{
    public class Oidb0x570_8 : Service
    {
        private Oidb0x570_8()
        {
            Register("OidbSvc.0x570_8", this);
        }

        public static Service Instance { get; } = new Oidb0x570_8();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 3)
                return false;

            if (args[0] is uint groupUin
                && args[1] is uint memberUin
                && args[2] is uint timeSeconds)
                return Request_0x570_8(core, groupUin, memberUin, timeSeconds);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        private bool Request_0x570_8(Core core, uint groupUin,
            uint memberUin, uint timeSeconds)
        {
            var oidbPacket = new OidbCmd0x570_8(groupUin, memberUin, timeSeconds);
            core.SsoMan.PostMessage(this, oidbPacket);

            return true;
        }

    }
}

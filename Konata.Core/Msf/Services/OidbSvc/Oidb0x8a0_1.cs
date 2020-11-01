using System;
using Konata.Msf.Packets.Oidb;

namespace Konata.Msf.Services.OidbSvc
{
    public class Oidb0x8a0_1 : Service
    {
        private Oidb0x8a0_1()
        {
            Register("OidbSvc.0x8a0_1", this);
        }

        public static Service Instance { get; } = new Oidb0x8a0_1();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                return false;

            if (args.Length != 3)
                return false;

            if (args[0] is uint groupUin
                && args[1] is uint memberUin
                && args[2] is bool preventRequest)
                return Request_0x8a0_1(core, groupUin, memberUin, preventRequest);

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            return false;
        }

        private bool Request_0x8a0_1(Core core, uint groupUin,
            uint memberUin, bool preventRequest)
        {
            var oidbPacket = new OidbCmd0x8a0_1(groupUin, memberUin, preventRequest);
            core.SsoMan.PostMessage(this, oidbPacket);

            return true;
        }
    }
}

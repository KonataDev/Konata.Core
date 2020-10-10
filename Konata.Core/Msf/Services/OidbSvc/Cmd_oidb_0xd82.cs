using System;

namespace Konata.Msf.Services.OidbSvc
{
    internal class Cmd_oidb_0xd82 : Service
    {
        private Cmd_oidb_0xd82()
        {
            Register("OidbSvc.oidb_0xd82", this);
        }

        public static Service Instance { get; } = new Cmd_oidb_0xd82();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            return false;
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }
    }
}

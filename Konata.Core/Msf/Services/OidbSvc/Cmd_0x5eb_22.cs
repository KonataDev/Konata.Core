using System;

namespace Konata.Msf.Services.OidbSvc
{
    internal class Cmd_0x5eb_22 : Service
    {
        private Cmd_0x5eb_22()
        {
            Register("OidbSvc.0x5eb_22", this);
        }

        public static Service Instance { get; } = new Cmd_0x5eb_22();

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

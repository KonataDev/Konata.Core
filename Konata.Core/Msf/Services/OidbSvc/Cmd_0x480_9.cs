using System;

namespace Konata.Msf.Services.OidbSvc
{
    public class Cmd_0x480_9 : Service
    {
        private Cmd_0x480_9()
        {
            Register("OidbSvc.0x480_9", this);
        }

        public static Service Instance { get; } = new Cmd_0x480_9();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }
    }
}

using System;

namespace Konata.Msf.Services.MessageSvc
{
    public class PushForceOffline : Service
    {
        private PushForceOffline()
        {
            Register("MessageSvc.PushForceOffline", this);
        }

        public static Service Instance { get; } = new PushForceOffline();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

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

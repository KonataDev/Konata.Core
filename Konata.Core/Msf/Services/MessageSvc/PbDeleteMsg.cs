using System;

namespace Konata.Msf.Services.MessageSvc
{
    internal class PbDeleteMsg : Service
    {
        private PbDeleteMsg()
        {
            Register("MessageSvc.PbDeleteMsg", this);
        }

        public static Service Instance { get; } = new PbDeleteMsg();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

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

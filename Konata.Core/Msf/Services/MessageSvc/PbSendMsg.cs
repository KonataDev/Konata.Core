using System;

namespace Konata.Msf.Services.MessageSvc
{
    internal class PbSendMsg : Service
    {
        private PbSendMsg()
        {
            Register("MessageSvc.PbSendMsg", this);
        }

        public static Service Instance { get; } = new PbSendMsg();

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

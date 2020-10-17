using System;

namespace Konata.Msf.Services.MessageSvc
{
    internal class PushRead : Service
    {
        private PushRead()
        {
            Register("MessageSvc.PushReaded", this);
        }

        public static Service Instance { get; } = new PushRead();

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

using System;

namespace Konata.Services.MessageSvc
{
    public class RequestPushStatus : Service
    {
        private RequestPushStatus()
        {
            Register("MessageSvc.RequestPushStatus", this);
        }

        public static Service Instance { get; } = new RequestPushStatus();

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

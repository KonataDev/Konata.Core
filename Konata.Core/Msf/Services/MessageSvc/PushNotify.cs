using System;

namespace Konata.Msf.Services.MessageSvc
{
    public class PushNotify : Service
    {
        private PushNotify()
        {
            Register("MessageSvc.PushNotify", this);
        }

        public static Service Instance { get; } = new PushNotify();

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

        private bool Handle_Notify(Core core)
        {
            // TODO: MessageSvc.PbGetMsg
            return true;
        }
    }
}

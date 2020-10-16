using System;

namespace Konata.Msf.Services.PbMessageSvc
{
    internal class PbMessageWithDraw : Service
    {
        private PbMessageWithDraw()
        {
            Register("MessageSvc.PbMessageWithDraw", this);
        }

        public static Service Instance { get; } = new PbMessageWithDraw();

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

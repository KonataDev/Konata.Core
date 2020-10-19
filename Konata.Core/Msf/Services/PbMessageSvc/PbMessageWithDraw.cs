using System;

namespace Konata.Msf.Services.PbMessageSvc
{
    public class PbMessageWithDraw : Service
    {
        private PbMessageWithDraw()
        {
            Register("PbMessageSvc.PbMessageWithDraw", this);
        }

        public static Service Instance { get; } = new PbMessageWithDraw();

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

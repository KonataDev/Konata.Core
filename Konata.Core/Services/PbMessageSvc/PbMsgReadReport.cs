using System;

namespace Konata.Services.PbMessageSvc
{
    public class PbMsgReadReport : Service
    {
        private PbMsgReadReport()
        {
            Register("PbMessageSvc.PbMsgReadedReport", this);
        }

        public static Service Instance { get; } = new PbMsgReadReport();

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

        private bool Request_PbMsgReadedReport(Core core,
            uint fromUin, uint sentTime, byte[] syncCookie)
        {

            return false;
        }
    }
}

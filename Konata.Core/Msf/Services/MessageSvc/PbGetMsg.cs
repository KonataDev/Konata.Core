using System;
using Konata.Library.Protobuf;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Msf.Services.MessageSvc
{
    public class PbGetMsg : Service
    {
        private PbGetMsg()
        {
            Register("MessageSvc.PbGetMsg", this);
        }

        public static Service Instance { get; } = new PbGetMsg();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_PbGetMsg(core);
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }

        private bool Request_PbGetMsg(Core core)
        {
            var sequence = core.SsoMan.GetNewSequence();
            var request = new ProtoGetMsg(core.SigInfo.SyncCookie);

            core.SsoMan.PostMessage(this, request.Serialize(), sequence);

            return true;
        }

        private bool Handle_PbGetMsg(Core core)
        {
            return true;
        }
    }
}

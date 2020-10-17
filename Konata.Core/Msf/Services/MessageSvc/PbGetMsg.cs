using System;
using Konata.Library.Protobuf;
using Konata.Msf.Packets.Protobuf;

namespace Konata.Msf.Services.MessageSvc
{
    internal class PbGetMsg : Service
    {
        private PbGetMsg()
        {
            Register("MessageSvc.PbGetMsg", this);
        }

        public static Service Instance { get; } = new PbGetMsg();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_GetMsg(core);
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }

        private bool Request_GetMsg(Core core)
        {
            var sequence = core._ssoMan.GetNewSequence();
            var request = new ProtoGetMsg(core._sigInfo._syncCookie);

            core._ssoMan.PostMessage(this, ProtoSerializer.Serialize(request), sequence);

            return true;
        }
    }
}

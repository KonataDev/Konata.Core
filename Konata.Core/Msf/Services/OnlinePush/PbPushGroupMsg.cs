using System;
using System.IO;

namespace Konata.Msf.Services.OnlinePush
{
    public class PbPushGroupMsg : Service
    {
        private PbPushGroupMsg()
        {
            Register("OnlinePush.PbPushGroupMsg", this);
        }

        public static Service Instance { get; } = new PbPushGroupMsg();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return Handle_PbPushGroupMsg(core,
                ((Packet)args[0]).TakeAllBytes(out byte[] _));
        }

        private bool Handle_PbPushGroupMsg(Core core, byte[] pbData)
        {

            return true;
        }
    }
}

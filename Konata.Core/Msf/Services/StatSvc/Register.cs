using System;
using Konata.Msf.Packets.Svc;
using Konata.Msf.Packets.Wup.UniPacket;

namespace Konata.Msf.Services.StatSvc
{
    internal class Register : Service
    {
        private Register()
        {
            Register("StatSvc.register", this);
        }

        public static Service Instance { get; } = new Register();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            if (method == "")
                throw new Exception("???");

            return Request_Register(core);
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return Handle_Register(core);
        }

        private bool Handle_Register(Core core)
        {
            return false;
        }

        private bool Request_Register(Core core)
        {
            var uniPacket = new UniPacket(true);
            uniPacket.SetServantName(name);
            uniPacket.SetFuncName("SvcReqRegister");

            var request = new SvcReqRegister();
            
            return false;
        }
    }
}

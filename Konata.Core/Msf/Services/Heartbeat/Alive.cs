using System;

namespace Konata.Msf.Services.Heartbeat
{
    internal class Alive : Service
    {
        private Alive()
        {
            Register("Heartbeat.Alive", this);
        }

        public static Service Instance { get; } = new Alive();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_Heartbeat(core);
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            return Handle_Heartbeat(core);
        }

        private bool Handle_Heartbeat(Core core)
        {
            // TODO: refresh heartbeat timer or task
            return true;
        }

        private bool Request_Heartbeat(Core core)
        {
            var request = new Packet();
            var sequence = core._ssoMan.GetNewSequence();
            core._ssoMan.PostMessage(this, request, sequence);

            return false;
        }
    }
}

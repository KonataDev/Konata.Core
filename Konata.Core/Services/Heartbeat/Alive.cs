using System;

namespace Konata.Services.Heartbeat
{
    public class Alive : Service
    {
        private Alive()
        {
            Register("Heartbeat.Alive", this);
        }

        public static Service Instance { get; } = new Alive();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_Heartbeat(core);
        }

        public override bool OnHandle(Core core, params object[] args)
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
            //var request = new Packet();
            //var sequence = core.SsoMan.GetNewSequence();
            //core.SsoMan.PostMessage(this, request, sequence);

            return false;
        }
    }
}

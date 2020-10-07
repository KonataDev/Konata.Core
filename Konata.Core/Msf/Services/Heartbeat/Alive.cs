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
            switch (method)
            {
                default: return false;
            }
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }
    }
}

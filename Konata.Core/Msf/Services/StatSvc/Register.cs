using System;

namespace Konata.Msf.Services.Wtlogin
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

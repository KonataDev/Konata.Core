using System;

namespace Konata.Msf.Services.Wtlogin
{
    class Trans_emp : Service
    {
        private Trans_emp()
        {
            Register("wtlogin.trans_emp", this);
        }

        public static Service Instance { get; } = new Trans_emp();

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

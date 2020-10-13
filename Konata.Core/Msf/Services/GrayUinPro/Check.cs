using System;

namespace Konata.Msf.Services.GrayUinPro
{
    internal class Check : Service
    {
        private Check()
        {
            Register("GrayUinPro.Check", this);
        }

        public static Service Instance { get; } = new Check();

        protected override bool OnRun(Core core, string method, params object[] args)
        {

            return false;
        }

        protected override bool OnHandle(Core core, params object[] args)
        {

            return false;
        }

    }
}

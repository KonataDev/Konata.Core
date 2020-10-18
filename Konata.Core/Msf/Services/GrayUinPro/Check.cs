using System;

namespace Konata.Msf.Services.GrayUinPro
{
    public class Check : Service
    {
        private Check()
        {
            Register("GrayUinPro.Check", this);
        }

        public static Service Instance { get; } = new Check();

        public override bool OnRun(Core core, string method, params object[] args)
        {

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {

            return false;
        }

    }
}

using System;
using Konata.Msf;

namespace Konata.Msf.Services.GrayUinPro
{
    internal class Check : Service
    {
        static Check()
        {
            Register("GrayUinPro.Check", new Check());
        }

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

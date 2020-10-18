using System;

namespace Konata.Msf.Services.Wtlogin
{
    public class Trans_emp : Service
    {
        private Trans_emp()
        {
            Register("wtlogin.trans_emp", this);
        }

        public static Service Instance { get; } = new Trans_emp();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            switch (method)
            {
                case "Request_Transport":
                    return Request_Transport(core);
                default: return false;
            }
        }

        public override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return false;
        }

        /// <summary>
        /// 請求發送報告 沒什麽用 暫時咕
        /// </summary>
        /// <param name="core"></param>
        /// <returns></returns>
        private bool Request_Transport(Core core)
        {
            return false;
        }
    }
}

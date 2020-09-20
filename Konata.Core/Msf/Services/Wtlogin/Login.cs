using System;
using Konata.Msf;
using Konata.Msf.Packets.Oicq;

namespace Konata.Msf.Services.Wtlogin
{
    internal class Login : Service
    {
        private Login()
        {
            Register("wtlogin.login", this);
        }

        public static Service Instance { get; } = new Login();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            switch (method)
            {
                case "Request_TGTGT": return Request_TGTGT(core);
                default: return false;
            }
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            var packet = ((Packet)args[0]);
            var oicqRequest = new OicqRequest(packet.TakeAllBytes(out byte[] _));

            Console.WriteLine($"  [oicqRequest] oicqCommand => {oicqRequest._oicqCommand}");
            Console.WriteLine($"  [oicqRequest] oicqVersion => {oicqRequest._oicqVersion}");
            Console.WriteLine($"  [oicqRequest] oicqStatus => {oicqRequest._oicqStatus}");

            core._oicqStatus = oicqRequest._oicqStatus;

            switch (oicqRequest._oicqStatus)
            {
                case OicqStatus.DoVerifySlider:
                    return Handle_VerifySliderCaptcha(core, oicqRequest);
                case OicqStatus.PreventByIncorrectUserOrPwd:
                    return Handle_InvalidUserOrPassword(core, oicqRequest);
            }

            return false;
        }

        /// <summary>
        /// 請求 OicqRequestTgtgt
        /// </summary>
        /// <param name="core"></param>
        internal bool Request_TGTGT(Core core)
        {
            var sequence = core._ssoMan.GetNewSequence();

            var request = new OicqRequestTgtgt(
                core._uin, core._password, sequence, core._keyRing);

            core._ssoMan.PostMessage(this, request);

            return true;
        }

        internal bool Handle_VerifySliderCaptcha(Core core, OicqRequest request)
        {

            return false;
        }

        internal bool Handle_InvalidUserOrPassword(Core core, OicqRequest request)
        {
            Console.WriteLine("Incorrect account or password.");
            return false;
        }

    }
}

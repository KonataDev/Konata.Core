using System;
using Konata.Msf;
using Konata.Msf.Packets.Oicq;
using Konata.Msf.Packets.Tlv;

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
                case "Request_TGTGT":
                    return Request_TGTGT(core);
                case "Request_SliderCaptcha":
                    return Request_SliderCaptcha(core, (string)args[0], (string)args[1]);
                case "Request_SmsCaptcha":
                    return Request_SmsCaptcha(core, (string)args[0], (string)args[1]);
                default: return false;
            }
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            var packet = (Packet)args[0];
            var oicqRequest = new OicqRequest(packet.TakeAllBytes(out byte[] _),
                core._keyRing._shareKey);

            Console.WriteLine($"  [oicqRequest] oicqCommand => {oicqRequest._oicqCommand}");
            Console.WriteLine($"  [oicqRequest] oicqVersion => {oicqRequest._oicqVersion}");
            Console.WriteLine($"  [oicqRequest] oicqStatus => {oicqRequest._oicqStatus}");

            core._oicqStatus = oicqRequest._oicqStatus;
            switch (oicqRequest._oicqStatus)
            {
                case OicqStatus.DoVerifySlider:
                    return Handle_VerifySliderCaptcha(core, oicqRequest);
                case OicqStatus.DoVerifyDeviceLockViaSms:
                    return Handle_VerifySmsCaptcha(core, oicqRequest);
                case OicqStatus.PreventByIncorrectUserOrPwd:
                    return Handle_InvalidUserOrPassword(core, oicqRequest);
                default: Handle_UnknownOicqRequest(core, oicqRequest); break;
            }

            return false;
        }

        #region Event Requests

        /// <summary>
        /// 請求 OicqRequestTgtgt
        /// </summary>
        /// <param name="core"></param>
        internal bool Request_TGTGT(Core core)
        {
            Console.WriteLine("Submit OicqRequestTGTGT.");

            var sequence = core._ssoMan.GetNewSequence();
            var request = new OicqRequestTgtgt(core._uin, sequence, core._keyRing);

            core._ssoMan.PostMessage(this, request, sequence);

            return true;
        }

        /// <summary>
        /// 請求 OicqRequestCheckImage
        /// </summary>
        /// <param name="core"></param>
        /// <param name="sigSission"></param>
        /// <param name="sigTicket"></param>
        /// <returns></returns>
        internal bool Request_SliderCaptcha(Core core,
            string sigSission, string sigTicket)
        {
            Console.WriteLine("Submit OicqRequestCheckImage.");

            var sequence = core._ssoMan.GetNewSequence();
            var request = new OicqRequestCheckImage(core._uin, core._keyRing,
                sigSission, sigTicket);

            core._ssoMan.PostMessage(this, request, sequence);

            return true;
        }

        /// <summary>
        /// 請求 OicqRequestCheckSms
        /// </summary>
        /// <param name="core"></param>
        /// <param name="sigSission"></param>
        /// <param name="sigAnswer"></param>
        /// <returns></returns>
        internal bool Request_SmsCaptcha(Core core, string sigSission, string sigAnswer)
        {
            Console.WriteLine("Submit OicqRequestCheckISms.");

            var sequence = core._ssoMan.GetNewSequence();
            // <TODO> OicqRequestCheckSms

            return true;
        }

        #endregion

        #region Event Handlers

        internal bool Handle_VerifySliderCaptcha(Core core, OicqRequest request)
        {
            Console.WriteLine("Do slider verification.");

            var tlvs = request._oicqRequestBody.TakeAllBytes(out var _);
            var unpacker = new TlvUnpacker(tlvs, true);

            Tlv tlv104 = unpacker.TryGetTlv(0x104);
            Tlv tlv192 = unpacker.TryGetTlv(0x192);
            if (tlv104 != null && tlv192 != null)
            {
                var sig = ((T104Body)tlv104._tlvBody)._sigSession;
                var captcha = ((T192Body)tlv192._tlvBody)._url;

                Console.WriteLine($"  SigSession => {sig}");
                Console.WriteLine($"  CaptchaUrl => {captcha}");
                Console.WriteLine($"Please input ticket:");

                core.PostEvent(EventType.VerifySliderCaptcha, sig, captcha);
            }
            return false;
        }

        internal bool Handle_VerifySmsCaptcha(Core core, OicqRequest request)
        {
            Console.WriteLine("Do sms verification.");

            core.PostEvent(EventType.VerifySmsCaptcha);

            return false;
        }

        internal bool Handle_VerifyImageCaptcha(Core core, OicqRequest request)
        {
            Console.WriteLine("Do image verification.");

            core.PostEvent(EventType.VerifySmsCaptcha);

            return false;
        }

        internal bool Handle_VerifyDeviceLock(Core core, OicqRequest request)
        {
            return false;
        }

        internal bool Handle_InvalidUserOrPassword(Core core, OicqRequest request)
        {
            Console.WriteLine("[Error] Incorrect account or password.");
            return false;
        }

        internal bool Handle_UnknownOicqRequest(Core core, OicqRequest request)
        {
            Console.WriteLine("[Error] Unknown OicqRequest received.");
            return false;
        }

        #endregion
    }
}

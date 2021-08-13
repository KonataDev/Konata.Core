using System;

namespace Konata.Core.Events.Model
{
    public class WtLoginEvent : ProtocolEvent
    {
        public enum Type
        {
            /// <summary>
            ///     <b>[Out]</b> Wtlogin failed about unknown issue
            /// </summary>
            Unknown,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin failed about not implemented functions
            /// </summary>
            NotImplemented,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin OK
            /// </summary>
            OK,

            /// <summary>
            ///     <b>[In]</b> Wtlogin start
            /// </summary>
            Tgtgt,

            /// <summary>
            ///     <b>[In] [Out]</b> Wtlogin do/submit SMS captcha
            /// </summary>
            CheckSms,

            /// <summary>
            ///     <b>[In] [Out]</b> Wtlogin do/submit slider captcha
            /// </summary>
            CheckSlider,

            /// <summary>
            ///     <b>[In] [Out]</b> Wtlogin do/submit devlock
            /// </summary>
            CheckDevLock,

            /// <summary>
            ///     <b>[In]</b> Wtlogin request refresh SMS
            /// </summary>
            RefreshSMS,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin denied from server
            /// </summary>
            LoginDenied,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin failed about pwd issue
            /// </summary>
            InvalidUinOrPassword,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin failed about login environment
            /// </summary>
            InvalidLoginEnvironment,

            /// <summary>
            ///     <b>[Out]</b> Wtlogin failed about invalid SMS code
            /// </summary>
            InvalidSmsCode,
        }

        public Type EventType { get; set; }

        public string SmsPhone { get; set; }

        public string SmsCountry { get; set; }

        public string SliderURL { get; set; }

        public string CaptchaResult { get; set; }

        public WtLoginEvent()
            => WaitForResponse = true;
    }

    public class CaptchaEvent : BaseEvent
    {
        public enum CaptchaType
        {
            Unknown,
            SMS,
            Slider
        }

        public CaptchaType Type
            => _captchaType;

        public string SliderUrl
            => _sliderUrl;

        public string Phone
            => _smsPhone;

        public string Country
            => _smsCountry;

        private CaptchaType _captchaType;
        private string _sliderUrl;
        private string _smsPhone;
        private string _smsCountry;

        public CaptchaEvent(WtLoginEvent wtEvent)
        {
            switch (wtEvent.EventType)
            {
                case WtLoginEvent.Type.CheckSms:
                    _smsPhone = wtEvent.SmsPhone;
                    _smsCountry = wtEvent.SmsCountry;
                    _captchaType = CaptchaType.SMS;
                    break;

                case WtLoginEvent.Type.CheckSlider:
                    _sliderUrl = wtEvent.SliderURL;
                    _captchaType = CaptchaType.Slider;
                    break;
            }
        }
    }
}

using System;

namespace Konata.Core.Event.EventModel
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
            CheckSMS,

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
}

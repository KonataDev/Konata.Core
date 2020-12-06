using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventWtLogin : KonataEventArgs
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

        public class Info
        {
            public uint Age { get; set; }

            public uint Face { get; set; }

            public string Name { get; set; }
        }

        public Type EventType { get; set; }

        public string WtLoginSession { get; set; }

        public string WtLoginSmsToken { get; set; }

        public string WtLoginSmsPhone { get; set; }

        public string WtLoginSmsCountry { get; set; }

        public string WtLoginSliderURL { get; set; }

        public string WtLoginCaptchaResult { get; set; }

        public byte[] TgtKey { get; set; }

        public byte[] TgtToken { get; set; }

        public byte[] D2Key { get; set; }

        public byte[] D2Token { get; set; }

        public byte[] GtKey { get; set; }

        public byte[] StKey { get; set; }

        public byte[] WtSessionTicketSig { get; set; }

        public byte[] WtSessionTicketKey { get; set; }

        public Info UinInfo { get; set; }
    }
}

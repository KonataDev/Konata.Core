// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Konata.Core.Events.Model
{
    public class WtLoginEvent : ProtocolEvent
    {
        public enum Type
        {
            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin failed with unknown issue
            /// </summary>
            Unknown,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin failed about not implemented functions
            /// </summary>
            NotImplemented,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin OK
            /// </summary>
            OK,

            /// <summary>
            /// <b>[In]</b> <br/>
            /// Wtlogin start
            /// </summary>
            Tgtgt,

            /// <summary>
            /// <b>[In]</b> <br/>
            /// Wtlogin xchg
            /// </summary>
            Xchg,

            /// <summary>
            /// <b>[In] [Out]</b> <br/>
            /// Wtlogin do/submit SMS captcha
            /// </summary>
            CheckSms,

            /// <summary>
            /// <b>[In] [Out]</b> <br/>
            /// Wtlogin do/submit slider captcha
            /// </summary>
            CheckSlider,

            /// <summary>
            /// <b>[In] [Out]</b> <br/>
            /// Wtlogin do/submit devlock
            /// </summary>
            VerifyDeviceLock,

            /// <summary>
            /// <b>[In]</b> <br/>
            /// Wtlogin request refresh sms
            /// </summary>
            RefreshSms,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Refresh sms failed
            /// </summary>
            RefreshSmsFailed,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin denied from server
            /// </summary>
            LoginDenied,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin failed about pwd issue
            /// </summary>
            InvalidUinOrPassword,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin failed about login environment
            /// </summary>
            HighRiskEnvironment,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// Wtlogin failed about invalid SMS code
            /// </summary>
            InvalidSmsCode,

            /// <summary>
            /// <b>[Out]</b> <br/>
            /// token expired [xchg only]
            /// </summary>
            TokenExpired,
        }

        public Type EventType { get; }

        public string SmsPhone { get; }

        public string SmsCountry { get; }

        public string SliderUrl { get; }

        public string CaptchaResult { get; }

        private WtLoginEvent(Type eventType)
            : base(6000, true)
        {
            EventType = eventType;
        }

        private WtLoginEvent(Type eventType, string captcha)
            : base(6000, true)
        {
            EventType = eventType;
            CaptchaResult = captcha;
        }

        private WtLoginEvent(int resultCode, string sliderUrl)
            : base(resultCode)
        {
            EventType = Type.CheckSlider;
            SliderUrl = sliderUrl;
        }

        private WtLoginEvent(int resultCode, string smsPhone,
            string smsCountry) : base(resultCode)
        {
            EventType = Type.CheckSms;
            SmsPhone = smsPhone;
            SmsCountry = smsCountry;
        }

        private WtLoginEvent(int resultCode, Type eventType,
            string reason) : base(resultCode)
        {
            EventType = eventType;
            EventMessage = reason;
        }

        /// <summary>
        /// Construct tgtgt request
        /// </summary>
        /// <returns></returns>
        internal static WtLoginEvent CreateTgtgt()
            => new(Type.Tgtgt);

        /// <summary>
        /// Construct xchg request
        /// </summary>
        /// <returns></returns>
        internal static WtLoginEvent CreateXchg()
            => new(Type.Xchg);

        /// <summary>
        /// Construct refresh sms request
        /// </summary>
        /// <returns></returns>
        internal static WtLoginEvent CreateRefreshSms()
            => new(Type.RefreshSms);

        /// <summary>
        /// Construct submit ticket request
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        internal static WtLoginEvent CreateSubmitTicket(string ticket)
            => new(Type.CheckSlider, ticket);

        /// <summary>
        /// Construct submit sms code request
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static WtLoginEvent CreateSubmitSmsCode(string code)
            => new(Type.CheckSms, code);

        /// <summary>
        /// Construct check device lock request
        /// </summary>
        /// <returns></returns>
        internal static WtLoginEvent CreateCheckDevLock()
            => new(Type.VerifyDeviceLock);

        /// <summary>
        /// Construct wtlogin ok result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static WtLoginEvent ResultOk(int resultCode)
            => new(resultCode, Type.OK, "wtLogin ok");

        /// <summary>
        /// Construct wtlogin device lock result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static WtLoginEvent ResultVerifyDeviceLock(int resultCode)
            => new(resultCode, Type.VerifyDeviceLock, "wtLogin verify device lock");

        /// <summary>
        /// Construct check slider result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="sliderUrl"></param>
        /// <returns></returns>
        internal static WtLoginEvent ResultCheckSlider(int resultCode,
            string sliderUrl) => new(resultCode, sliderUrl);

        /// <summary>
        /// Construct check sms result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="smsPhone"></param>
        /// <param name="smsCountry"></param>
        /// <returns></returns>
        internal static WtLoginEvent ResultCheckSms(int resultCode, string smsPhone,
            string smsCountry) => new(resultCode, smsPhone, smsCountry);

        /// <summary>
        /// Construct refresh sms result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static WtLoginEvent ResultRefreshSms(int resultCode)
            => new(resultCode, Type.RefreshSms, "Do refresh sms");

        internal static WtLoginEvent ResultInvalidUsrPwd(int resultCode)
            => new(resultCode, Type.InvalidUinOrPassword, "Incorrect account or password.");

        internal static WtLoginEvent ResultInvalidSmsCode(int resultCode)
            => new(resultCode, Type.InvalidSmsCode, "Incorrect sms code.");

        internal static WtLoginEvent ResultHighRiskEnvironment(int resultCode, string reason)
            => new(resultCode, Type.HighRiskEnvironment, reason);

        internal static WtLoginEvent ResultLoginDenied(int resultCode, string reason)
            => new(resultCode, Type.LoginDenied, reason);

        internal static WtLoginEvent ResultTokenExpired(int resultCode)
            => new(resultCode, Type.TokenExpired, "Token expired.");

        internal static WtLoginEvent ResultUnknown(int resultCode, string reason)
            => new(resultCode, Type.LoginDenied, reason);

        internal static WtLoginEvent ResultNotImplemented(int resultCode, string reason)
            => new(Type.NotImplemented) {ResultCode = resultCode, EventMessage = reason, WaitForResponse = false, Timeout = 0};
    }

    public class CaptchaEvent : BaseEvent
    {
        public enum CaptchaType
        {
            Unknown,
            SMS,
            Slider
        }

        public CaptchaType Type { get; }

        public string SliderUrl { get; }

        public string Phone { get; }

        public string Country { get; }

        private CaptchaEvent(WtLoginEvent wtEvent)
        {
            switch (wtEvent.EventType)
            {
                case WtLoginEvent.Type.CheckSms:
                    Type = CaptchaType.SMS;
                    Phone = wtEvent.SmsPhone;
                    Country = wtEvent.SmsCountry;
                    break;

                case WtLoginEvent.Type.CheckSlider:
                    Type = CaptchaType.Slider;
                    SliderUrl = wtEvent.SliderUrl;
                    break;
            }
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="wtEvent"></param>
        /// <returns></returns>
        internal static CaptchaEvent Create(WtLoginEvent wtEvent)
            => new(wtEvent);
    }
}

using System;

namespace Konata.Events
{
    public class EventWtLoginExchange : EventParacel
    {
        public enum EventType
        {
            Tgtgt,
            RefreshSms,
            CheckSmsCaptcha,
            CheckImageCaptcha,
            CheckSliderCaptcha,
        }

        public EventType Type { get; set; }

        /// <summary>
        /// Captcha results
        /// </summary>
        public string CaptchaResult { get; set; }

        /// <summary>
        /// For EventType.CheckSliderCaptcha
        /// </summary>
        public string SliderUrl { get; set; }

        /// <summary>
        /// For EventType.CheckSmsCaptcha
        /// </summary>
        public string SmsPhoneNumber { get; set; }

        /// <summary>
        /// For EventType.CheckSmsCaptcha
        /// </summary>
        public string SmsPhoneCountryCode { get; set; }
    }
}

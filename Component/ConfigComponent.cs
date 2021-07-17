using System;
using System.Text;

using Konata.Core.Service;

namespace Konata.Core.Component
{
    [Component("ConfigComponent", "Konata Config Management Component")]
    public class ConfigComponent : BaseComponent
    {
        public SignInfo SignInfo { get; private set; }

        public BotDevice DeviceInfo { get; private set; }

        public BotConfig GlobalConfig { get; private set; }

        public void LoadSignInfo(SignInfo signinfo)
            => SignInfo = signinfo;

        public void LoadConfig(BotConfig config)
            => GlobalConfig = config;

        public void LoadDeviceInfo(BotDevice device)
            => DeviceInfo = device;

        /// <summary>
        /// Sync cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void SyncCookie(byte[] cookie)
            => SignInfo.Account.SyncCookie = cookie;
    }
}

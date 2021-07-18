using System;
using System.Text;

namespace Konata.Core.Component
{
    [Component("ConfigComponent", "Konata Config Management Component")]
    public class ConfigComponent : BaseComponent
    {
        public BotKeyStore KeyStore { get; private set; }

        public BotDevice DeviceInfo { get; private set; }

        public BotConfig GlobalConfig { get; private set; }

        public void LoadKeyStore(BotKeyStore keyStore, string imei)
        {
            KeyStore = keyStore;
            KeyStore.Initial(imei);
        }

        public void LoadConfig(BotConfig config)
            => GlobalConfig = config;

        public void LoadDeviceInfo(BotDevice device)
            => DeviceInfo = device;

        /// <summary>
        /// Sync cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void SyncCookie(byte[] cookie)
            => KeyStore.Account.SyncCookie = cookie;
    }
}

using System;
using System.Text;
using Konata.Core.Attributes;

// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Model
{
    [Component("ConfigComponent", "Konata Config Management Component")]
    internal class ConfigComponent : InternalComponent
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

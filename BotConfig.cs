// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core
{
    public class BotConfig
    {
        public bool UseIPv6Connection { get; private set; }

        public bool ReConnectWhileLinkDown { get; private set; }

        public uint ReConnectTryCount { get; private set; }

        #region Custom Server IP

        public string CustomHost { get; private set; }

        #endregion

        #region Image upload

        public int ImageChunkSize { get; private set; }

        #endregion

        /// <summary>
        /// Get a default config
        /// </summary>
        /// <returns></returns>
        public static BotConfig Default()
        {
            return new BotConfig
            {
                UseIPv6Connection = false,
                ReConnectWhileLinkDown = true,
                ReConnectTryCount = 3,
                CustomHost = null,
                ImageChunkSize = 4096,
            };
        }
    }
}

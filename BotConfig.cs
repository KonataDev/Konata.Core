// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core
{
    public class BotConfig
    {
        public bool UseIPv6Connection { get; set; }

        public bool ReConnectWhileLinkDown { get; set; }

        public uint ReConnectTryCount { get; set; }

        #region Custom Server IP

        public string CustomHost { get; set; }

        #endregion

        #region Image upload

        public int ImageChunkSize { get; set; }

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

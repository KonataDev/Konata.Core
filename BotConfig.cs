// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core
{
    /// <summary>
    /// Config
    /// </summary>
    public class BotConfig
    {
        /// <summary>
        /// <b>[not used]</b> <br/>
        /// Use ipv6 connection.
        /// </summary>
        public bool UseIPv6Connection { get; set; }

        /// <summary>
        /// Reconnect while link down
        /// </summary>
        public bool ReConnectWhileLinkDown { get; set; }

        /// <summary>
        /// How many retry times while link down 
        /// </summary>
        public uint ReConnectTryCount { get; set; }

        #region Custom Server IP

        /// <summary>
        /// Custom the server
        /// </summary>
        public string CustomHost { get; set; }

        #endregion

        #region Image upload

        /// <summary>
        /// Image chunk size while uploading
        /// </summary>
        public int ImageChunkSize { get; set; }

        #endregion

        #region Audio

        /// <summary>
        /// Is enable audio function
        /// </summary>
        public bool EnableAudio { get; set; }

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
                EnableAudio = false
            };
        }
    }
}

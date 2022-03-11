// ReSharper disable PossibleNullReferenceException
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Common
{
    /// <summary>
    /// Config
    /// </summary>
    public class BotConfig
    {
        #region Networking

        /// <summary>
        /// <b>[not used]</b> <br/>
        /// Use ipv6 connection.
        /// </summary>
        public bool UseIPv6Connection { get; set; }

        /// <summary>
        /// Try reconnect while offline.
        /// </summary>
        public bool TryReconnect { get; set; }

        /// <summary>
        /// Custom the server
        /// </summary>
        public string CustomHost { get; set; }

        /// <summary>
        /// Highway chunk size
        /// </summary>
        public int HighwayChunkSize { get; set; }

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
                TryReconnect = true,
                CustomHost = null,
                HighwayChunkSize = 4096,
                EnableAudio = false
            };
        }
    }
}

// ReSharper disable PossibleNullReferenceException
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Konata.Core.Common;

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
    /// Custom server
    /// </summary>
    public string CustomHost { get; set; }

    /// <summary>
    /// Highway chunk size
    /// </summary>
    public int HighwayChunkSize { get; set; }

    /// <summary>
    /// Default net timeout
    /// </summary>
    public int DefaultTimeout { get; set; }

    #endregion

    #region Audio

    /// <summary>
    /// Is enable audio function
    /// </summary>
    public bool EnableAudio { get; set; }

    #endregion

    #region Protocol

    /// <summary>
    /// Procotol type
    /// </summary>
    public OicqProtocol Protocol;

    #endregion

    /// <summary>
    /// Get a default config
    /// </summary>
    /// <returns></returns>
    public static BotConfig Default()
    {
        return new BotConfig
        {
            TryReconnect = true,
            CustomHost = null,
            HighwayChunkSize = 4096,
            EnableAudio = false,
            DefaultTimeout = 6000,
            Protocol = OicqProtocol.AndroidPhone
        };
    }
}

public enum OicqProtocol
{
    AndroidPhone = 0,
    Watch = 1,
    Ipad = 2,
    AndroidPad = 3,
}

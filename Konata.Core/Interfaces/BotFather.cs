using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Konata.Core.Interfaces;

#pragma warning disable CS0618
public static class BotFather
{
    /// <summary>
    /// Create a bot instance
    /// </summary>
    /// <param name="config"></param>
    /// <param name="device"></param>
    /// <param name="keystore"></param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static Bot Create(BotConfig config, BotDevice device, BotKeyStore keystore)
    {
        var bot = new Bot(config, device, keystore);
        return bot;
    }

    /// <summary>
    /// Create a bot instance
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="passwd"></param>
    /// <param name="config"></param>
    /// <param name="device"></param>
    /// <param name="keystore"></param>
    /// <param name="protocol"></param>
    /// <returns></returns>
    [KonataApi(1, experimental: true)]
    public static Bot Create(string uin, string passwd, out BotConfig config, out BotDevice device,
                             out BotKeyStore keystore, OicqProtocol protocol = OicqProtocol.AndroidPhone)
    {
        device = BotDevice.Default();
        config = BotConfig.Default();
        config.Protocol = protocol;
        keystore = new BotKeyStore(uin, passwd);
        return new Bot(config, device, keystore);
    }
}

#pragma warning restore CS0618

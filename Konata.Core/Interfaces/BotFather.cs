using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Components;

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
    public static Bot Create(BotConfig config, BotDevice device, BotKeyStore keystore)
    {
        var bot = new Bot(config, device, keystore);
        return bot;
    }
}

#pragma warning restore CS0618

using System.Collections.Generic;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Packets;

namespace Konata.Core.Components.Services;

/// <summary>
/// SSO Service interface
/// </summary>
internal interface IService
{
    /// <summary>
    /// Parse packet to protocol event
    /// </summary>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="output"></param>
    /// <param name="extra"></param>
    /// <returns></returns>
    bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output, out List<ProtocolEvent> extra);

    /// <summary>
    /// Build packet
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="device"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    bool Build(int sequence, ProtocolEvent input, BotKeyStore keystore, BotDevice device, ref PacketBase output);
}

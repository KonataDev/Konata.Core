using System.Collections.Generic;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Packets;

// ReSharper disable InvertIf
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Components.Services;

internal abstract class BaseService<TEvent> : IService
    where TEvent : ProtocolEvent
{
    /// <summary>
    /// Parse packet
    /// </summary>
    /// <param name="input"></param>
    /// <param name="appInfo"></param>
    /// <param name="keystore"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected virtual bool Parse(SSOFrame input, AppInfo appInfo, 
        BotKeyStore keystore, out TEvent output) => (output = null) != null;

    /// <summary>
    /// Parse packet
    /// </summary>
    /// <param name="input"></param>
    /// <param name="appInfo"></param>
    /// <param name="keystore"></param>
    /// <param name="output"></param>
    /// <param name="extra"></param>
    /// <returns></returns>
    protected virtual bool Parse(SSOFrame input, AppInfo appInfo, 
        BotKeyStore keystore, out TEvent output, List<ProtocolEvent> extra)
    {
        output = null;
        if (Parse(input, appInfo, keystore, out var x))
        {
            output = x;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="input"></param>
    /// <param name="appInfo"></param>
    /// <param name="keystore"></param>
    /// <param name="device"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected virtual bool Build(int sequence, TEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output) => false;

    bool IService.Build(int sequence, ProtocolEvent input, AppInfo appInfo, BotKeyStore keystore,
        BotDevice device, ref PacketBase output) => Build(sequence, (TEvent) input, appInfo, keystore, device, ref output);

    bool IService.Parse(SSOFrame input, AppInfo appInfo, BotKeyStore keystore, out ProtocolEvent output, out List<ProtocolEvent> extra)
    {
        output = null;
        extra = new List<ProtocolEvent>();

        if (Parse(input, appInfo, keystore, out var x, extra)) output = x;
        return output != null;
    }
}

using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Packets;

// ReSharper disable InvertIf
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Services;

internal abstract class BaseService<TEvent> : IService
    where TEvent : ProtocolEvent
{
    /// <summary>
    /// Parse packet
    /// </summary>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected virtual bool Parse(SSOFrame input, BotKeyStore keystore,
        out TEvent output) => (output = null) != null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="device"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected virtual bool Build(int sequence, TEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output) => false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="device"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    bool IService.Build(int sequence, ProtocolEvent input, BotKeyStore keystore,
        BotDevice device, ref PacketBase output) => Build(sequence, (TEvent) input, keystore, device, ref output);

    /// <summary>
    /// Parse packet
    /// </summary>
    /// <param name="input"></param>
    /// <param name="keystore"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    bool IService.Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
    {
        output = null;

        if (Parse(input, keystore, out var dirtyCs)) output = dirtyCs;
        return output != null;
    }
}

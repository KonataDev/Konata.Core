using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;

namespace Konata.Core.Components.Services.Client;

[EventSubscribe(typeof(CorrectTimeEvent))]
[Service("Client.CorrectTime", PacketType.TypeA, AuthFlag.DefaultlyNo, SequenceMode.Managed)]
internal class CorrectTime : BaseService<CorrectTimeEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out CorrectTimeEvent output)
    {
        output = CorrectTimeEvent.Result(input.Payload.TakeUintBE(out _));
        return true;
    }

    protected override bool Build(int sequence, CorrectTimeEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutUintBE(4);
        return true;
    }
}

using Konata.Core.Packets;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets.SvcPush;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[Service("MessageSvc.PushNotify", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushNotify : BaseService<PushNotifyEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PushNotifyEvent output)
    {
        input.Payload.EatBytes(4);
        var bytes = input.Payload.TakeAllBytes(out _);

        var packets = new SvcPushNotify(bytes);
        output = PushNotifyEvent.Push((NotifyType) packets.Type);
        return true;
    }
}

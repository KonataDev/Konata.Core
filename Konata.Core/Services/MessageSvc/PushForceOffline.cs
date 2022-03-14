using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MessageSvc;

[Service("MessageSvc.PushForceOffline", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushForceOffline : BaseService<OnlineStatusEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out OnlineStatusEvent output)
    {
        output = OnlineStatusEvent.Push
            (OnlineStatusEvent.Type.Offline, "MessageSvc.PushForceOffline");
        return true;
    }
}

using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[Service("MessageSvc.PushReaded", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushRead : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ProtocolEvent output)
    {
        output = null;
        return false;
    }
}

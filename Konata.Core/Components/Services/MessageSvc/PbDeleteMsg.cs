using System;
using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[Service("MessageSvc.PbDeleteMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbDeleteMsg : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ProtocolEvent output)
    {
        throw new NotImplementedException();
    }

    protected override bool Build(int sequence, ProtocolEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        throw new NotImplementedException();
    }
}

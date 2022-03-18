using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.LongConn;

[EventSubscribe(typeof(LongConnOffPicUpEvent))]
[Service("LongConn.OffPicUp", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class OffPicUp : BaseService<LongConnOffPicUpEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out LongConnOffPicUpEvent output)
    {
        throw new NotImplementedException();
    }

    protected override bool Build(int sequence, LongConnOffPicUpEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        throw new NotImplementedException();
    }
}

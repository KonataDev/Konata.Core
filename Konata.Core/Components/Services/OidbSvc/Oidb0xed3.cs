using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment
// ReSharper disable InconsistentNaming

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupPokeEvent))]
[EventSubscribe(typeof(FriendPokeEvent))]
[Service("OidbSvc.0xed3", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0xed3 : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out ProtocolEvent output)
    {
        var pb = ProtobufDecoder.Create(input.Payload);
        output = new ProtocolEvent((int) pb[3].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, ProtocolEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        switch (input)
        {
            case GroupPokeEvent gpe:
                output = new OidbCmd0xed3_0(gpe.GroupUin, gpe.MemberUin);
                return true;

            case FriendPokeEvent fpe:
                output = new OidbCmd0xed3_0(fpe.SelfUin, fpe.FriendUin, false);
                return true;
        }

        return false;
    }
}

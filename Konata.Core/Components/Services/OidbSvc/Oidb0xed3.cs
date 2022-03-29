using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupPokeEvent))]
[Service("OidbSvc.0xed3", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0xed3 : BaseService<GroupPokeEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out GroupPokeEvent output)
    {
        
        var pb = ProtobufDecoder.Create(input.Payload);
        output = GroupPokeEvent.Result((int) pb[3].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, GroupPokeEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0xed3(input.GroupUin, input.MemberUin);
        return true;
    }
}

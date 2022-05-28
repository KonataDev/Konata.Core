using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Components.Services.OnlinePush;

[Service("OnlinePush.PbPushTransMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbPushTransMsg : BaseService<PushTransMsgEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out PushTransMsgEvent output, List<ProtocolEvent> extra)
    {
        var pb = ProtobufDecoder.Create(input.Payload.GetBytes());
        var type = pb[3].AsNumber();
        var buf = pb[10].AsBuffer();
        var svrip = pb[11].AsNumber();
        buf.TakeUintBE(out var groupUin);
        buf.EatBytes(1);

        if (type == 44)
        {
            buf.TakeByte(out var type2);
            if (type2 == 0 || type2 == 1)
            {
                buf.TakeUintBE(out var memberUin);
                buf.TakeByte(out var set);
                extra.Add(GroupPromoteAdminEvent.Push(groupUin, memberUin, set > 0));
            }
        }
        else if (type == 34)
        {
            buf.TakeUintBE(out var memberUin);
            buf.TakeByte(out var type2);
            var dismiss = false;
            uint operatorUin = 0;
            if (type2 == 0x82 || type2 == 0x2)
            {
                operatorUin = memberUin;
            }
            else
            {
                if (type2 == 0x1 || type2 == 0x81)
                    dismiss = true;
                buf.TakeUintBE(out operatorUin);
            }

            extra.Add(GroupKickMemberEvent.Push(groupUin, memberUin, operatorUin, dismiss));
        }

        output = PushTransMsgEvent.Push(input.Sequence, (int) svrip);
        return true;
    }
}

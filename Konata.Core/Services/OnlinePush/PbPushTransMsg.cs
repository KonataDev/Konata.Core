using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Services.OnlinePush;

[Service("OnlinePush.PbPushTransMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbPushTransMsg : BaseService<PushTransMsgEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PushTransMsgEvent output)
    {
        ProtocolEvent innerEvent = null;
        var tree = new ProtoTreeRoot(input.Payload.GetBytes(), true);
        var type = tree.GetLeafVar("18");
        var buf = new ByteBuffer(tree.GetLeafBytes("52"));
        var svrip = tree.GetLeafVar("58");
        buf.TakeUintBE(out var groupUin);
        buf.EatBytes(1);

        if (type == 44)
        {
            buf.TakeByte(out var type2);
            if (type2 == 0 || type2 == 1)
            {
                buf.TakeUintBE(out var memberUin);
                buf.TakeByte(out var set);
                innerEvent = GroupPromoteAdminEvent.Push(groupUin, memberUin, set > 0);
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
            innerEvent = GroupKickMemberEvent.Push(groupUin, memberUin, operatorUin, dismiss);
        }

        output = PushTransMsgEvent.Push(innerEvent, input.Sequence, (int) svrip);

        return true;
    }
}

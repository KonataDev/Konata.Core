using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.PbMessageSvc;

[EventSubscribe(typeof(GroupMessageReadEvent))]
[Service("PbMessageSvc.PbMsgReadedReport", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.EventBased)]
internal class PbMsgReadReport : BaseService<GroupMessageReadEvent>
{
    protected override bool Build(int sequence, GroupMessageReadEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new GroupMsgReadReport(input.GroupUin, input.RequestId));
        return true;
    }
}

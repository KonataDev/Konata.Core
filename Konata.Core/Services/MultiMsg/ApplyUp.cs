using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MultiMsg;

[EventSubscribe(typeof(MultiMsgApplyUpEvent))]
[Service("MultiMsg.ApplyUp", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class ApplyUp : BaseService<MultiMsgApplyUpEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out MultiMsgApplyUpEvent output)
    {
        var root = ProtoTreeRoot.Deserialize(input.Payload.GetBytes(), true);
        root = root.GetLeaf<ProtoTreeRoot>("12");
        {
            var result = root.GetLeafVar("08");
            var host = root.GetLeaves<ProtoVarInt>("20")[0];
            var port = root.GetLeaves<ProtoVarInt>("28")[0];
            var ticket = root.GetLeafBytes("52");
            var msgResid = root.GetLeafString("12");
            var msguKey = root.GetLeafBytes("1A");

            // Construct result
            output = MultiMsgApplyUpEvent.Result((int) result, new MultiMsgUpInfo
            {
                Ip = host,
                Host = NetTool.UintToIPBE((uint) host),
                Port = port,
                UploadTicket = ticket,
                MsgResId = msgResid,
                MsgUKey = msguKey
            });
        }
        return true;
    }

    protected override bool Build(int sequence, MultiMsgApplyUpEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new MultiMsgApplyUpReq
            (input.DestUin, input.PackedLength, input.Md5Hash));
        return true;
    }
}

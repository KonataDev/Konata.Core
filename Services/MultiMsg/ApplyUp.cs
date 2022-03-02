using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Services.MultiMsg;

[EventSubscribe(typeof(MultiMsgApplyUpEvent))]
[Service("MultiMsg.ApplyUp", "Request multimsg upload")]
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

    protected override bool Build(Sequence sequence, MultiMsgApplyUpEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var applyReq = new MultiMsgApplyUpReq(input.DestUin, input.PackedLength, input.Md5Hash);

        if (SSOFrame.Create("MultiMsg.ApplyUp", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(applyReq), out var ssoFrame))
        {
            if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
            {
                return ServiceMessage.Build(toService, device, out output);
            }
        }

        return false;
    }
}

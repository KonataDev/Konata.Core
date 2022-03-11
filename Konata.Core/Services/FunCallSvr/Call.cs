using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Services.FunCallSvr;

[EventSubscribe(typeof(FunCallSvrCallEvent))]
[Service("FunCallSvr.call", "Calling service")]
internal class Call : BaseService<FunCallSvrCallEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out FunCallSvrCallEvent output)
    {
        var root = ProtoTreeRoot.Deserialize
            (input.Payload.GetBytes(), true);
        {
            var result = (int) root.GetLeafVar("08");
            output = FunCallSvrCallEvent.Result(result);
        }
        return true;
    }

    protected override bool Build(Sequence sequence, FunCallSvrCallEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var req = new ProtoTreeRoot();
        {
            req.AddLeafVar("08", 2);
            req.AddLeafVar("10", 109);
            req.AddLeafString("1A", AppInfo.ApkVersionName);
            req.AddTree("2A", _ =>
            {
                _.AddLeafVar("08", input.CallUin);
                _.AddLeafVar("12", 0);
            });
        }

        if (SSOFrame.Create("FunCallSvr.call", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(req), out var ssoFrame))
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

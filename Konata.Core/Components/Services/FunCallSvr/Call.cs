using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Components.Services.FunCallSvr;

[EventSubscribe(typeof(FunCallSvrCallEvent))]
[Service("FunCallSvr.call", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
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

    protected override bool Build(int sequence, FunCallSvrCallEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
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
        output.PutProtoNode(req);
        return true;
    }
}

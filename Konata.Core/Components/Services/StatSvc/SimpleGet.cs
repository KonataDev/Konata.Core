using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets;
using Konata.Core.Events.Model;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.StatSvc;

[EventSubscribe(typeof(SimpleGetEvent))]
[Service("StatSvc.SimpleGet", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Session)]
internal class SimpleGet : BaseService<SimpleGetEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out SimpleGetEvent output)
    {
        var root = ProtoTreeRoot.Deserialize
            (input.Payload.GetBytes(), true);
        {
            output = SimpleGetEvent.Result(
                (int) root.GetLeafVar("08"),
                (int) root.GetLeafVar("18"),
                (int) root.GetLeafVar("28"),
                root.GetLeafString("22")
            );
            return true;
        }
    }

    protected override bool Build(int sequence, SimpleGetEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output) => true;
}

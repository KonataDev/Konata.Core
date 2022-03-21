using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.Trpc.Group_pro.SyncLogic.SyncLogic;

[EventSubscribe(typeof(GuildSyncFirstView))]
[Service("trpc.group_pro.synclogic.SyncLogic.SyncFirstView", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class SyncFirstView : BaseService<GuildSyncFirstView>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GuildSyncFirstView output)
    {
        var pb = ProtobufDecoder.Create(input.Payload);
        output = GuildSyncFirstView.Result(pb[6].AsNumber());
        return true;
    }

    protected override bool Build(int sequence, GuildSyncFirstView input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        var root = new ProtoTreeRoot();
        {
            root.AddLeafVar("08", 0);
            root.AddLeafVar("10", 0);
            root.AddLeafVar("18", 0);
        }
        output.PutProtoNode(root);
        return true;
    }
}

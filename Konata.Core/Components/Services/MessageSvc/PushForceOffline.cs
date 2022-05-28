using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets.SvcPush;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[Service("MessageSvc.PushForceOffline", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushForceOffline : BaseService<ForceOfflineEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ForceOfflineEvent output)
    {
        var tree = new SvcPushForceOffline(input.Payload.GetBytes());
        output = new ForceOfflineEvent(tree.Title, tree.Message);
        return true;
    }
}

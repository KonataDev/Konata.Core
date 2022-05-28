using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets.SvcPush;

namespace Konata.Core.Components.Services.ConfigPushSvc;

[Service("ConfigPushSvc.PushReq", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushReq : BaseService<PushConfigEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out PushConfigEvent output)
    {
        output = null;

        var push = new SvcPushConfig(input.Payload.GetBytes());
        {
            if (push.ServerList.Count == 0) return false;

            output = PushConfigEvent.Push(push.ServerList[0].Host,
                push.ServerList[0].Port, push.Ticket);
        }

        return true;
    }
}

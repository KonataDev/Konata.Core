using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets.SvcPush;

namespace Konata.Core.Services.ConfigPushSvc;

[Service("ConfigPushSvc.PushReq", "Push req")]
internal class PushReq : BaseService<PushConfigEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PushConfigEvent output)
    {
        output = null;

        var push = new SvcPushConfig(input.Payload.GetBytes());
        {
            if (push.ServerList.Count == 0) return false;

            output = PushConfigEvent.Push(push.ServerList[0].Host,
                push.ServerList[1].Port, push.Ticket);
        }

        return true;
    }
}

using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushForceOffline", "Force offline")]
    public class PushForceOffline : BaseService<OnlineStatusEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out OnlineStatusEvent output)
        {
            output = OnlineStatusEvent.Push
                (OnlineStatusEvent.Type.Offline, "MessageSvc.PushForceOffline");
            return true;
        }
    }
}

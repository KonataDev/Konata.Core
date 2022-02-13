using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushNotify", "Push notify on received a private message")]
    public class PushNotify : BaseService<PrivateMessageNotifyEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out PrivateMessageNotifyEvent output)
        {
            output = PrivateMessageNotifyEvent.Push();
            return true;
        }
    }
}

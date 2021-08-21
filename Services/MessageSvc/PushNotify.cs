using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushNotify", "Push notify on received a private message")]
    public class PushNotify : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
            => (output = PrivateMessageNotifyEvent.Push()) == output;

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}

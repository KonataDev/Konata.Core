using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PushForceOffline", "Force offline")]
    public class PushForceOffline : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            output = OnlineStatusEvent.Push
                (OnlineStatusEvent.Type.Offline, "MessageSvc.PushForceOffline");
            return true;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = 0;
            return false;
        }
    }
}

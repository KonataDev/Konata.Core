using Konata.Core.Events;
using Konata.Core.Packets;

namespace Konata.Core.Services
{
    public abstract class BaseService : IService
    {
        public virtual bool Parse(SSOFrame input,
            BotKeyStore keystore, out ProtocolEvent output)
        {
            output = null;
            return false;
        }

        public virtual bool Build(Sequence sequence, ProtocolEvent input, BotKeyStore
            keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            newSequence = 0;
            output = null;
            return false;
        }

        public virtual bool Build(PacketBase buffer, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device)
        {
            return false;
        }
    }
}

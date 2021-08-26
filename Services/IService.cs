using Konata.Core.Events;
using Konata.Core.Packets;

namespace Konata.Core.Services
{
    /// <summary>
    /// SSO Service interface
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Parse packet to protocol event
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output);

        /// <summary>
        /// Build binary packet
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="device"></param>
        /// <param name="newSequence"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output);

        // TODO:
        // bool Build(WupBuffer buffer, ProtocolEvent input, BotKeyStore keystore, BotDevice device);
    }
}

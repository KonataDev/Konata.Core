using Konata.Core.Events;
using Konata.Core.Packets;

// ReSharper disable InvertIf

namespace Konata.Core.Services
{
    public abstract class BaseService<TEvent> : IService
        where TEvent : ProtocolEvent
    {
        /// <summary>
        /// Parse packet
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected virtual bool Parse(SSOFrame input, 
            BotKeyStore keystore, out TEvent output)
        {
            output = null;
            return false;
        }

        /// <summary>
        /// Build packet
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="device"></param>
        /// <param name="newSequence"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected virtual bool Build(Sequence sequence, TEvent input, BotKeyStore
            keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            newSequence = 0;
            output = null;
            return false;
        }

        /// <summary>
        /// Build packet
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="device"></param>
        /// <param name="newSequence"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool IService.Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (TEvent) input, keystore, device, out newSequence, out output);

        /// <summary>
        /// Parse packet
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool IService.Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            output = null;
            
            if (Parse(input, keystore, out var dirtyCs))
            {
                output = dirtyCs;
                return true;
            }

            return false;
        }

        /// <summary>
        /// TODO: sso service refactor
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="input"></param>
        /// <param name="keystore"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public virtual bool Build(PacketBase buffer, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device)
        {
            return false;
        }
    }
}

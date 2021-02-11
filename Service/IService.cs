using System;
using System.Text;

using Konata.Core.Event;
using Konata.Core.Packet;

namespace Konata.Core.Service
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
        /// <param name="signInfo"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output);

        /// <summary>
        /// Build binary packet
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="input"></param>
        /// <param name="signInfo"></param>
        /// <param name="newSequence"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output);
    }
}

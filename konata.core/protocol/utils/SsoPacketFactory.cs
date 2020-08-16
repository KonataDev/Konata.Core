using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata.Protocol.Utils
{
    public static class SsoPacketFactory
    {
        private static uint _ssoSequence = 85600;
        private static uint _ssoSessionId = 0xBCA2DA01;

        public static SsoPacket Build(SsoCommand command, OicqRequest request)
        {
            ++_ssoSequence;
            return new SsoPacket(_ssoSequence, _ssoSessionId, command, request);
        }
    }
}

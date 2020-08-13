using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konata.Protocol.Packet;

namespace Konata.Protocol.Utils
{
    public static class ToServicePacketFactory
    {

        public static ToServicePacket Build(SsoPacket ssoPacket, uint packetType, uint encryptType, byte[] encryptKey, long uin)
        {
            return new ToServicePacket(ssoPacket, packetType, encryptType, encryptKey, uin);
        }

        public static ToServicePacket Build(SsoPacket ssoPacket, uint packetType, uint encryptType, long uin)
        {
            return new ToServicePacket(ssoPacket, packetType, encryptType, uin);
        }
    }
}

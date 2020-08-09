using System;
using System.Linq;
using System.Text;
using Konata.Utils;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;

namespace Konata.Protocol.Utils
{
    public class SsoBuilder
    {
        private readonly OicqRequest oicqRequest;

        public SsoBuilder(OicqRequest request)
        {
            oicqRequest = request;
        }

        public PacketBase GetPacket()
        {
            ToServicePacket packet = new ToServicePacket();
            packet.SetBytes(oicqRequest.GetBytes());
            return packet;
        }

    }
}

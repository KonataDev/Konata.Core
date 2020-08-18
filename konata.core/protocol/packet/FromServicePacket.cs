using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Protocol.Packet
{
    public class FromServicePacket
    {

        private PacketBase _packet;

        public FromServicePacket(byte[] fromServiceBytes)
        {
            _packet.SetBytes(fromServiceBytes);
        }
    }
}

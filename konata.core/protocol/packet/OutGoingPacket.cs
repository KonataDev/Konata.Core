using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Protocol.Packet
{
    class OutGoingPacket : PacketBase
    {
        private byte[] data;

        public override byte[] GetBytes() => data;

        public override void SetBytes(byte[] bytes) => data = bytes;

    }
}

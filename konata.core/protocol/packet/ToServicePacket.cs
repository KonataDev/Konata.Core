using System;

namespace Konata.Protocol.Packet
{
    public class ToServicePacket : PacketBase
    {
        private byte[] data;

        public override byte[] GetBytes() => data;

        public override void SetBytes(byte[] bytes) => data = bytes;

    }
}

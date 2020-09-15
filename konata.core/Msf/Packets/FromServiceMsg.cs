using System;

namespace Konata.Msf.Packets
{
    public class FromServiceMsg : Packet
    {
        public readonly uint _length;
        public readonly uint _packetType;
        public readonly uint _encryptType;
        public readonly byte _unknownZero;
        public readonly string _uinString;

        public FromServiceMsg(byte[] data)
        {

        }
    }
}

using System;

namespace Konata.Msf.Packets
{
    public class FromServiceMessage : Packet
    {
        public readonly uint _length;
        public readonly uint _packetType;
        public readonly byte _encryptType;
        public readonly byte _unknownZero;
        public readonly string _uinString;

        public FromServiceMessage(byte[] data) : base(data)
        {
            TakeUintBE(out _length);
            TakeUintBE(out _packetType);
            TakeByte(out _encryptType);
            TakeByte(out _unknownZero);
            TakeString(out _uinString, 4);
        }
    }
}

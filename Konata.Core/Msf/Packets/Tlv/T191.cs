using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T191Body : TlvBody
    {
        public readonly byte _unknownK;

        public T191Body(byte unknownK = 0x82)
            : base()
        {
            _unknownK = unknownK;

            PutByte(_unknownK);
        }

        public T191Body(byte[] data)
            : base(data)
        {
            TakeByte(out _unknownK);
        }
    }
}

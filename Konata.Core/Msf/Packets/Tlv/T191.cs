using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T191 : TlvBase
    {
        public T191(byte unknownK = 0x82)
            : base(0x0191, new T191Body(unknownK))
        {

        }
    }

    public class T191Body : TlvBody
    {
        public readonly byte _unknownK;

        public T191Body(byte unknownK)
            : base()
        {
            PutByte(_unknownK);
        }

        public T191Body(byte[] data)
            : base(data)
        {
            TakeByte(out _unknownK);
        }
    }
}

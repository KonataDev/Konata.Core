using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T166 : TlvBase
    {
        public T166(byte imgType)
            : base(0x0166, new T166Body(imgType))
        {

        }
    }

    public class T166Body : TlvBody
    {
        public readonly byte _imgType;

        public T166Body(byte imgType)
            : base()
        {
            _imgType = imgType;

            PutByte(_imgType);
        }

        public T166Body(byte[] data)
            : base(data)
        {
            TakeByte(out _imgType);
        }
    }
}

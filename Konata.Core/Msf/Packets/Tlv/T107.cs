using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T107 : TlvBase
    {
        public T107(ushort picType, byte capType = 0, ushort picSize = 0, byte retType = 1)
            : base(0x0107, new T107Body(picType, capType, picSize, retType))
        {

        }
    }

    public class T107Body : TlvBody
    {
        public readonly ushort _picType;
        public readonly byte _capType;
        public readonly ushort _picSize;
        public readonly byte _retType;

        public T107Body(ushort picType, byte capType, ushort picSize, byte retType)
            : base()
        {
            _picType = picType;
            _capType = capType;
            _picSize = picSize;
            _retType = retType;

            PutUshortBE(_picType);
            PutByte(_capType);
            PutUshortBE(_picSize);
            PutByte(_retType);
        }

        public T107Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _picType);
            TakeByte(out _capType);
            TakeUshortBE(out _picSize);
            TakeByte(out _retType);
        }
    }
}

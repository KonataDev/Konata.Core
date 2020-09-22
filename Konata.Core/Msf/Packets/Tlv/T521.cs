using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T521 : TlvBase
    {
        public T521(uint productType = 0, ushort unknown = 0)
            : base(0x0521, new T521Body(productType, unknown))
        {

        }
    }

    public class T521Body : TlvBody
    {
        public readonly uint _productType;
        public readonly ushort _unknown;

        public T521Body(uint productType, ushort unknown)
            : base()
        {
            _productType = productType;
            _unknown = unknown;

            PutUintBE(_productType);
            PutUshortBE(_unknown);
        }

        public T521Body(byte[] data)
            : base(data)
        {
            TakeUintBE(out _productType);
            TakeUshortBE(out _unknown);
        }
    }
}

using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T521Body : TlvBody
    {
        public readonly uint _productType;
        public readonly ushort _unknown;

        public T521Body(uint productType = 0, ushort unknown = 0)
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

using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T191Body : TlvBody
    {
        public readonly byte _verifyType;

        public T191Body(byte verifyType = 0x82)
            : base()
        {
            _verifyType = verifyType;

            PutByte(_verifyType);
        }

        public T191Body(byte[] data)
            : base(data)
        {
            TakeByte(out _verifyType);
        }
    }
}

using System;

namespace Konata.Packets.Tlv
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

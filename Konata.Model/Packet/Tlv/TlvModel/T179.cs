using System;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T179Body : TlvBody
    {
        public readonly ushort _verifyUrlLen;

        public T179Body(ushort verifyUrlLen)
            : base()
        {
            _verifyUrlLen = verifyUrlLen;

            PutUshortBE(_verifyUrlLen);
        }

        public T179Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _verifyUrlLen);
        }
    }
}

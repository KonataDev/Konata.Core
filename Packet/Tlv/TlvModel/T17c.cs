using System;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T17cBody : TlvBody
    {
        public readonly string _smsCode;

        public T17cBody(string smsCode)
            : base()
        {
            _smsCode = smsCode;

            PutString(_smsCode, Prefix.Uint16);
        }

        public T17cBody(byte[] data)
            : base(data)
        {
            TakeString(out _smsCode, Prefix.Uint16);
        }
    }
}

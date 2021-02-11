using System;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T17aBody : TlvBody
    {
        public readonly uint _smsAppId;

        public T17aBody(uint smsAppId)
            : base()
        {
            _smsAppId = smsAppId;

            PutUintBE(_smsAppId);
        }

        public T17aBody(byte[] data)
            : base(data)
        {
            TakeUintBE(out _smsAppId);
        }
    }
}

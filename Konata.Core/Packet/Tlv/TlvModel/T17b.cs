using System;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T17bBody : TlvBody
    {
        public readonly ushort _smsChanceCount;
        public readonly ushort _smsCoolDownSec;

        public T17bBody(ushort smsChanceCount, ushort smsCoolDownSec)
            : base()
        {
            _smsChanceCount = smsChanceCount;
            _smsCoolDownSec = smsCoolDownSec;

            PutUshortBE(_smsChanceCount);
            PutUshortBE(_smsCoolDownSec);
        }

        public T17bBody(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _smsChanceCount);
            TakeUshortBE(out _smsCoolDownSec);
        }
    }
}

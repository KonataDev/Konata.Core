using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T525 : TlvBase
    {
        public T525(T536 t536)
            : base(0x0525, new T525Body(t536))
        {

        }
    }

    public class T525Body : TlvBody
    {
        public readonly T536 _t536;

        public T525Body(T536 t536)
            : base()
        {
            _t536 = t536;

            PutUshortBE(1);
            PutTlv(_t536);
        }

        public T525Body(byte[] data)
            : base(data)
        {
            EatBytes(1);
            _t536 = new T536(TakeAllBytes(out var _));
        }
    }
}

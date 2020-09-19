using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T525 : TlvBase
    {
        private readonly T536 _t536;

        public T525(T536 t536) : base()
        {
            _t536 = t536;
            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x525);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(1);
            PutBytes(_t536.GetBytes());
        }
    }
}

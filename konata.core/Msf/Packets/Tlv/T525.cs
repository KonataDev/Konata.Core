using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T525 : TlvBase
    {
        private readonly T536 _t536;

        public T525(T536 t536)
        {
            _t536 = t536;
        }

        public override void PutTlvCmd()
        {
            return 0x525;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(1);
            builder.PutBytes(_t536.GetBytes(), 2);
            return builder.GetBytes();
        }
    }
}

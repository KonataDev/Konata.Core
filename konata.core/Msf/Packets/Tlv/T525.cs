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

        public override ushort GetTlvCmd()
        {
            return 0x525;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(1);
            builder.PutBytes(_t536.GetBytes(), false);
            return builder.GetBytes();
        }
    }
}

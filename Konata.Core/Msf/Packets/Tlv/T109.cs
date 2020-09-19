using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T109 : TlvBase
    {
        private readonly string _osType;

        public T109(string osType) : base()
        {
            _osType = osType;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x109);
        }

        public override void PutTlvBody()
        {
            PutString(_osType);
        }
    }
}

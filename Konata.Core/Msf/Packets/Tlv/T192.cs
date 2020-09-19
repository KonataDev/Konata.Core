using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T192 : TlvBase
    {
        private readonly string _url;

        public T192(string url) : base()
        {
            _url = url;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x192);
        }

        public override void PutTlvBody()
        {
            PutString(_url, 2);
        }
    }
}

using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T511 : TlvBase
    {
        private readonly string[] _domains;

        public T511(string[] domains)
        {
            _domains = domains;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0511);
        }

        public override void PutTlvBody()
        {
            PutUshortBE((ushort)_domains.Length);
            foreach (string element in _domains)
            {
                PutByte(0x01);
                PutString(element, 2);
            }
        }
    }
}

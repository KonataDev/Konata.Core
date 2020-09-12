using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T511 : TlvBase
    {
        private readonly string[] _domains;

        public T511(string[] domains)
        {
            _domains = domains;
        }

        public override ushort GetTlvCmd()
        {
            return 0x511;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUInt16((ushort)_domains.Length);
            foreach (string element in _domains)
            {
                builder.PutInt8(1);
                builder.PutString(element);
            }
            return builder.GetBytes();
        }
    }
}

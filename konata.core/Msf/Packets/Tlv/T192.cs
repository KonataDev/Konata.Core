using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T192 : TlvBase
    {
        private readonly string _url;

        public T192(string url)
        {
            _url = url;
        }

        public override ushort GetTlvCmd()
        {
            return 0x192;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushString(_url, false);
            return builder.GetBytes();
        }
    }
}

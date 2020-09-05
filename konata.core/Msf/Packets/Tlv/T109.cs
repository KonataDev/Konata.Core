using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T109 : TlvBase
    {
        private readonly string _osType;

        public T109(string osType)
        {
            _osType = osType;
        }

        public override ushort GetTlvCmd()
        {
            return 0x109;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushString(_osType, false);
            return builder.GetBytes();
        }
    }
}

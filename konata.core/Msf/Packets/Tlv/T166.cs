using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T166 : TlvBase
    {
        private readonly int _imgType;

        public T166(int imgType)
        {
            _imgType = imgType;
        }

        public override ushort GetTlvCmd()
        {
            return 0x166;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutByte((byte)_imgType);
            return builder.GetBytes();
        }
    }
}

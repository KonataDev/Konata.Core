using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T166 : TlvBase
    {
        private readonly int _imgType;

        public T166(int imgType) : base()
        {
            _imgType = imgType;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x166);
        }

        public override void PutTlvBody()
        {
            PutByte((byte)_imgType);
        }
    }
}

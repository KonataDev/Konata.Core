using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T153 : TlvBase
    {
        private readonly bool _isRooted;

        public T153(bool isRooted) : base()
        {
            _isRooted = isRooted;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x153);
        }

        public override void PutTlvBody()
        {
            PutBoolBE(_isRooted, 2);
        }
    }
}

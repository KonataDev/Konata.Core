using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T153 : TlvBase
    {
        private readonly bool _isRooted;

        public T153(bool isRooted)
        {
            _isRooted = isRooted;
        }

        public override void PutTlvCmd()
        {
            return 0x153;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBoolBE(_isRooted, 2);
            return builder.GetBytes();
        }
    }
}

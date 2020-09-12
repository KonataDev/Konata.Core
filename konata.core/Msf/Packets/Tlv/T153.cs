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

        public override ushort GetTlvCmd()
        {
            return 0x153;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBoolBE(_isRooted, 2);
            return builder.GetBytes();
        }
    }
}

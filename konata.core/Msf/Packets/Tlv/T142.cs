using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T142 : TlvBase
    {
        private const ushort _version = 0;

        private readonly string _apkId;

        public T142(string apkId)
        {
            _apkId = apkId;
        }

        public override ushort GetTlvCmd()
        {
            return 0x142;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_version);
            builder.PutString(_apkId, 2, 32);
            return builder.GetBytes();
        }
    }
}

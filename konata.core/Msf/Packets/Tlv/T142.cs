using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T142 : TlvBase
    {
        private const ushort _version = 0;

        private readonly string _apkId;

        public T142(string apkId)
        {
            _apkId = apkId;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x142);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_version);
            PutString(_apkId, 2, 32);
        }
    }
}

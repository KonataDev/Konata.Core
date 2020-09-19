using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class T187 : TlvBase
    {
        private readonly byte[] _macAddress;

        public T187(byte[] macAddress) : base()
        {
            _macAddress = macAddress;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x187);
        }

        public override void PutTlvBody()
        {
            PutEncryptedBytes(_macAddress, new Md5Cryptor(), null);
        }
    }
}

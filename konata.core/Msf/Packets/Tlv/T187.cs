using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T187 : TlvBase
    {
        private readonly byte[] _macAddress;

        public T187(byte[] macAddress)
        {
            _macAddress = macAddress;
        }

        public override void PutTlvCmd()
        {
            return 0x187;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(new Md5Cryptor().Encrypt(_macAddress));
            return builder.GetBytes();
        }
    }
}

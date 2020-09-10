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

        public override ushort GetTlvCmd()
        {
            return 0x187;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(new Md5Cryptor().Encrypt(_macAddress));
            return builder.GetBytes();
        }
    }
}

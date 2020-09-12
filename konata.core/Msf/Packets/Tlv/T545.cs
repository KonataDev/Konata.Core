using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T545 : TlvBase
    {
        private readonly string _unknownQiMei;

        public T545(string qiMei = "")
        {
            _unknownQiMei = qiMei;
        }

        public override ushort GetTlvCmd()
        {
            return 0x545;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(new Md5Cryptor().Encrypt(Hex.HexStr2Bytes(_unknownQiMei)), false);
            return builder.GetBytes();
        }
    }
}

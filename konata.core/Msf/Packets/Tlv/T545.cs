using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T545 : TlvBase
    {
        private readonly byte[] _unknownQiMei;

        public T545(string qiMei = "")
        {
            _unknownQiMei = Hex.HexStr2Bytes(qiMei);
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x545);
        }

        public override void PutTlvBody()
        {
            PutEncryptedBytes(_unknownQiMei, new Md5Cryptor(), null);
        }
    }
}

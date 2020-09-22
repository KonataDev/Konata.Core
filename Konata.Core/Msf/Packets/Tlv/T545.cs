using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class T545 : TlvBase
    {
        public T545(string qiMei = "")
            : base(0x0545, new T545Body(qiMei))
        {

        }
    }

    public class T545Body : TlvBody
    {
        public readonly byte[] _unknownQiMeiMd5;

        public T545Body(string qiMei)
            : base()
        {
            _unknownQiMeiMd5 = new Md5Cryptor().Encrypt(Hex.HexStr2Bytes(qiMei));

            PutBytes(_unknownQiMeiMd5);
        }

        public T545Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _unknownQiMeiMd5, Prefix.None);
        }
    }
}

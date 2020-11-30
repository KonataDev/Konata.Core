using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T545Body : TlvBody
    {
        public readonly byte[] _unknownQiMeiMd5;

        public T545Body(string qiMei = "")
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

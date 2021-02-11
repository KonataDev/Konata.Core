using Konata.Utils;
using Konata.Utils.IO;
using Konata.Utils.Crypto;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T188Body : TlvBody
    {
        public readonly byte[] _androidIdMd5;

        public T188Body(byte[] androidId, int androidIdLength)
            : base()
        {
            _androidIdMd5 = new Md5Cryptor().Encrypt(androidId);

            PutBytes(_androidIdMd5);
        }

        public T188Body(string androidId)
            : base()
        {
            _androidIdMd5 = new Md5Cryptor().Encrypt(ByteConverter.UnHex(androidId));

            PutBytes(_androidIdMd5);
        }

        public T188Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _androidIdMd5, Prefix.None);
        }
    }
}

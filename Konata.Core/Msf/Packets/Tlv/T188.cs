using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class T188 : TlvBase
    {
        public T188(byte[] androidId)
            : base(0x0188, new T188Body(androidId, androidId.Length))
        {

        }

        public T188(string androidId)
            : base(0x0188, new T188Body(androidId))
        {

        }
    }

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
            _androidIdMd5 = new Md5Cryptor().Encrypt(Hex.HexStr2Bytes(androidId));

            PutBytes(_androidIdMd5);
        }

        public T188Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _androidIdMd5, Prefix.None);
        }
    }
}

using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;

namespace Konata.Core.Packets.Tlv.Model;

internal class T545Body : TlvBody
{
    public readonly byte[] _unknownQiMeiMd5;

    public T545Body(string qiMei = "")
        : base()
    {
        _unknownQiMeiMd5 = new Md5Cryptor().Encrypt(ByteConverter.UnHex(qiMei));

        PutBytes(_unknownQiMeiMd5);
    }

    public T545Body(byte[] data)
        : base(data)
    {
        TakeBytes(out _unknownQiMeiMd5, Prefix.None);
    }
}

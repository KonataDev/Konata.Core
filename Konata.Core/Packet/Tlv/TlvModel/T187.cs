using System;
using Konata.Utils.Crypto;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T187Body : TlvBody
    {
        public readonly byte[] _macAddressMd5;

        public T187Body(byte[] macAddress, int macAddressLength)
            : base()
        {
            _macAddressMd5 = new Md5Cryptor().Encrypt(macAddress);

            PutBytes(_macAddressMd5);
        }

        public T187Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _macAddressMd5, Prefix.None);
        }
    }
}

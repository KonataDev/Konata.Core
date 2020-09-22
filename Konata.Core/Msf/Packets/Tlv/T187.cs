using System;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class T187 : TlvBase
    {
        public T187(byte[] macAddress)
            : base(0x0187, new T187Body(macAddress, macAddress.Length))
        {

        }
    }

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

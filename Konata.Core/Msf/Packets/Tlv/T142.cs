using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T142Body : TlvBody
    {
        public readonly ushort _version = 0;
        public readonly string _apkId;

        public T142Body(string apkId)
            : base()
        {
            _version = 0;
            _apkId = apkId;

            PutUshortBE(_version);
            PutString(_apkId, 2, 32);
        }

        public T142Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _version);
            TakeString(out _apkId, Prefix.Uint16);
        }
    }
}

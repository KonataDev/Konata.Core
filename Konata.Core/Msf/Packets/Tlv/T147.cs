using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T147 : TlvBase
    {
        public T147(uint appId, string apkVersionName, byte[] apkSignatureMd5)
            : base(0x0147, new T147Body(appId, apkVersionName, apkSignatureMd5))
        {

        }
    }

    public class T147Body : TlvBody
    {
        public readonly uint _appId;
        public readonly string _apkVersionName;
        public readonly byte[] _apkSignatureMd5;

        public T147Body(uint appId, string apkVersionName, byte[] apkSignatureMd5)
            : base()
        {
            _appId = appId;
            _apkVersionName = apkVersionName;
            _apkSignatureMd5 = apkSignatureMd5;

            PutUintBE(_appId);
            PutString(_apkVersionName, 2, 32);
            PutBytes(_apkSignatureMd5, 2, 32);
        }

        public T147Body(byte[] data)
            : base(data)
        {
            TakeUintBE(out _appId);
            TakeString(out _apkVersionName, Prefix.Uint16);
            TakeBytes(out _apkSignatureMd5, Prefix.Uint16);
        }
    }
}

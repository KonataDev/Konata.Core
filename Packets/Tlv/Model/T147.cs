using System;

namespace Konata.Core.Packets.Tlv.Model
{
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
            PutString(_apkVersionName, Prefix.Uint16, 32);
            PutBytes(_apkSignatureMd5, Prefix.Uint16, 32);
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

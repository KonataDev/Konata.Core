using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T148 : TlvBase
    {
        public T148(string appName, uint ssoVersion, uint appId, uint subAppId,
            string appVersion, string appSignature)
            : base(0x0148, new T148Body(appName, ssoVersion, appId, subAppId,
             appVersion, appSignature))
        {

        }
    }

    public class T148Body : TlvBody
    {
        public readonly string _appName;
        public readonly uint _ssoVersion;
        public readonly uint _appId;
        public readonly uint _subAppId;
        public readonly string _appVersion;
        public readonly string _appSignature;

        public T148Body(string appName, uint ssoVersion, uint appId, uint subAppId,
            string appVersion, string appSignature)
            : base()
        {
            _appName = appName;
            _ssoVersion = ssoVersion;
            _appId = appId;
            _subAppId = subAppId;
            _appVersion = appVersion;
            _appSignature = appSignature;

            PutString(_appName, 2, 32);
            PutUintBE(_ssoVersion);
            PutUintBE(_appId);
            PutUintBE(_subAppId);
            PutString(_appVersion, 2, 32);
            PutString(_appSignature, 2, 32);
        }

        public T148Body(byte[] data)
            : base(data)
        {
            TakeString(out _appName, Prefix.Uint16);
            TakeUintBE(out _ssoVersion);
            TakeUintBE(out _appId);
            TakeUintBE(out _subAppId);
            TakeString(out _appVersion, Prefix.Uint16);
            TakeString(out _appSignature, Prefix.Uint16);
        }
    }
}

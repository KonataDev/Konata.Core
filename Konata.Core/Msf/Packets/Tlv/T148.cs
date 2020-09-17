using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T148 : TlvBase
    {
        private readonly string _appName;
        private readonly uint _ssoVersion;
        private readonly uint _appId;
        private readonly uint _subAppId;
        private readonly string _appVersion;
        private readonly string _appSignature;

        public T148(string appName, uint ssoVersion, uint appId, uint subAppId,
            string appVersion, string appSignature)
        {
            _appName = appName;
            _ssoVersion = ssoVersion;
            _appId = appId;
            _subAppId = subAppId;
            _appVersion = appVersion;
            _appSignature = appSignature;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x148);
        }

        public override void PutTlvBody()
        {
            PutString(_appName);
            PutUintBE(_ssoVersion);
            PutUintBE(_appId);
            PutUintBE(_subAppId);
            PutString(_appVersion);
            PutString(_appSignature);
        }
    }
}

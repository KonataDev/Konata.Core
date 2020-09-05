using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T148 : TlvBase
    {
        private readonly string _appName;
        private readonly long _ssoVersion;
        private readonly long _appId;
        private readonly long _subAppId;
        private readonly string _appVersion;
        private readonly string _appSignature;

        public T148(string appName, long ssoVersion, long appId, long subAppId,
            string appVersion, string appSignature)
        {
            _appName = appName;
            _ssoVersion = ssoVersion;
            _appId = appId;
            _subAppId = subAppId;
            _appVersion = appVersion;
            _appSignature = appSignature;
        }

        public override ushort GetTlvCmd()
        {
            return 0x148;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushString(_appName);
            builder.PushInt32((int)_ssoVersion);
            builder.PushInt32((int)_appId);
            builder.PushInt32((int)_subAppId);
            builder.PushString(_appVersion);
            builder.PushString(_appSignature);
            return builder.GetBytes();
        }
    }
}

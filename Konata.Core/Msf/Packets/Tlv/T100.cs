using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T100 : TlvBase
    {
        private const ushort _dbBufVer = 1;
        private const int _ssoVer = 6;
        private const int _sigmap = 34869472;

        private readonly uint _appId;
        private readonly uint _subAppId;
        private readonly uint _appClientVersion;

        public T100(uint appId, uint subAppId, uint appClientVersion)
        {
            _appId = appId;
            _subAppId = subAppId;
            _appClientVersion = appClientVersion;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x100);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_dbBufVer);
            PutUintBE(_ssoVer);
            PutUintBE(_appId);
            PutUintBE(_subAppId);
            PutUintBE(_appClientVersion);
            PutUintBE(_sigmap);
        }
    }
}

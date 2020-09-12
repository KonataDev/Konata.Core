using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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

        public override ushort GetTlvCmd()
        {
            return 0x100;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_dbBufVer);
            builder.PutUintBE(_ssoVer);
            builder.PutUintBE(_appId);
            builder.PutUintBE(_subAppId);
            builder.PutUintBE(_appClientVersion);
            builder.PutUintBE(_sigmap);
            return builder.GetBytes();
        }
    }
}

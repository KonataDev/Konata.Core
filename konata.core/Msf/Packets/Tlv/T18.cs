using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T18 : TlvBase
    {
        private const ushort _sigVer = 0;
        private const ushort _pingVersion = 1;
        private const ushort _alwaysZero = 0;
        private const uint _ssoVersion = 1536;

        private readonly uint _appId;
        private readonly uint _appClientVersion;
        private readonly uint _uin;

        public T18(uint appId, uint appClientVersion, uint uin)
        {
            _appId = appId;
            _appClientVersion = appClientVersion;
            _uin = uin;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x18);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_pingVersion);
            PutUintBE(_ssoVersion);
            PutUintBE(_appId);
            PutUintBE(_appClientVersion);
            PutUintBE(_uin);
            PutUshortBE(_alwaysZero);
            PutUshortBE(0);
        }
    }
}

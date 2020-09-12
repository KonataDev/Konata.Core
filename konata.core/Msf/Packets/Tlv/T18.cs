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

        public override ushort GetTlvCmd()
        {
            return 0x18;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_pingVersion);
            builder.PutUintBE(_ssoVersion);
            builder.PutUintBE(_appId);
            builder.PutUintBE(_appClientVersion);
            builder.PutUintBE(_uin);
            builder.PutUshortBE(_alwaysZero);
            builder.PutUshortBE(0);
            return builder.GetBytes();
        }
    }
}

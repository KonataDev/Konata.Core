using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T18 : TlvBase
    {
        private const short _sigVer = 0;
        private const short _pingVersion = 1;
        private const short _alwaysZero = 0;
        private const int _ssoVersion = 1536;

        private readonly long _appId;
        private readonly int _appClientVersion;
        private readonly long _uin;

        public T18(long appId, int appClientVersion, long uin)
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
            builder.PutUshortBEBE(_pingVersion);
            builder.PutUintBE(_ssoVersion);
            builder.PutUintBE((int)_appId);
            builder.PutUintBE(_appClientVersion);
            builder.PutUintBE((int)_uin);
            builder.PutUshortBEBE(_alwaysZero);
            builder.PutUshortBE(0);
            return builder.GetBytes();
        }
    }
}

using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
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
            builder.PushInt16(_sigVer);
            builder.PushInt16(_pingVersion);
            builder.PushInt32(_ssoVersion);
            builder.PushInt32((int)_appId);
            builder.PushInt32(_appClientVersion);
            builder.PushInt32((int)_uin);
            builder.PushInt16(_alwaysZero);
            builder.PushInt16(0);
            return builder.GetPlainBytes();
        }
    }
}

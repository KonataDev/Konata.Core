using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T100 : TlvBase
    {
        private const short _dbBufVer = 1;
        private const int _ssoVer = 6;
        private const int _sigmap = 34869472;

        private readonly long _appId;
        private readonly long _subAppId;
        private readonly int _appClientVersion;

        public T100(long appId, long subAppId, int appClientVersion)
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
            builder.PushInt16(_dbBufVer);
            builder.PushInt32(_ssoVer);
            builder.PushInt32((int)_appId);
            builder.PushInt32((int)_subAppId);
            builder.PushInt32(_appClientVersion);
            builder.PushInt32(_sigmap);
            return builder.GetBytes();
        }
    }
}

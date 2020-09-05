using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T147 : TlvBase
    {
        private readonly long _appId;
        private readonly string _apkVersionName;
        private readonly byte[] _apkSignatureMd5;

        public T147(long appId, string apkVersionName, byte[] apkSignatureMd5)
        {
            _appId = appId;
            _apkVersionName = apkVersionName;
            _apkSignatureMd5 = apkSignatureMd5;
        }

        public override ushort GetTlvCmd()
        {
            return 0x147;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt32((int)_appId);
            builder.PushString(_apkVersionName, true, true, 32);
            builder.PushBytes(_apkSignatureMd5, false, true, true, 32);
            return builder.GetBytes();
        }
    }
}
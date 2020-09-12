using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T147 : TlvBase
    {
        private readonly uint _appId;
        private readonly string _apkVersionName;
        private readonly byte[] _apkSignatureMd5;

        public T147(uint appId, string apkVersionName, byte[] apkSignatureMd5)
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
            builder.PutUintBE(_appId);
            builder.PutString(_apkVersionName, 2, 32);
            builder.PutBytes(_apkSignatureMd5, 2, 32);
            return builder.GetBytes();
        }
    }
}
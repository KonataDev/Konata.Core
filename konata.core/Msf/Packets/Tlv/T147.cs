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

        public override void PutTlvCmd()
        {
            PutUshortBE(0x147);
        }

        public override void PutTlvBody()
        {
            PutUintBE(_appId);
            PutString(_apkVersionName, 2, 32);
            PutBytes(_apkSignatureMd5, 2, 32);
        }
    }
}
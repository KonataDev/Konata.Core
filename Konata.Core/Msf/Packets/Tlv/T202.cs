using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class T202 : TlvBase
    {
        private readonly byte[] _wifiBssid;
        private readonly string _wifiSsid;

        public T202(byte[] wifiBssid, string wifiSsid) : base()
        {
            _wifiBssid = wifiBssid;
            _wifiSsid = wifiSsid;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0202);
        }

        public override void PutTlvBody()
        {
            PutEncryptedBytes(_wifiBssid, new Md5Cryptor(), null, 2, 16);
            PutString(_wifiSsid, 2, 32);
        }
    }
}

using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T202 : TlvBase
    {
        private readonly byte[] _wifiBssid;
        private readonly string _wifiSsid;

        public T202(byte[] wifiBssid, string wifiSsid)
        {
            _wifiBssid = wifiBssid;
            _wifiSsid = wifiSsid;
        }

        public override void PutTlvCmd()
        {
            return 0x202;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(new Md5Cryptor().Encrypt(_wifiBssid), 2, 16);
            builder.PutString(_wifiSsid, 2, 32);
            return builder.GetBytes();
        }
    }
}

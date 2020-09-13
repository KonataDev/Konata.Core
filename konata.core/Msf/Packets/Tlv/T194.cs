using System.Text;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T194 : TlvBase
    {
        private readonly byte[] _imsi;

        public T194(string imsi)
        {
            _imsi = Encoding.UTF8.GetBytes(imsi);
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x194);
        }

        public override void PutTlvBody()
        {
            PutEncryptedBytes(_imsi, new Md5Cryptor(), null);
        }
    }
}

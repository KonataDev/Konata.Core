using System.Linq;
using System.Text;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T194 : TlvBase
    {
        private readonly string _imsi;

        public T194(string imsi)
        {
            _imsi = imsi;
        }

        public override void PutTlvCmd()
        {
            return 0x194;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(_imsi).ToArray()), 2);
            return builder.GetBytes();
        }
    }
}

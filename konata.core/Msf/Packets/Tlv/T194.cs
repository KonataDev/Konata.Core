using System.Linq;
using System.Text;
using Konata.Utils;
using Konata.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T194 : TlvBase
    {
        private readonly string _imsi;

        public T194(string imsi)
        {
            _imsi = imsi;
        }

        public override ushort GetTlvCmd()
        {
            return 0x194;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(_imsi).ToArray()), false);
            return builder.GetBytes();
        }
    }
}

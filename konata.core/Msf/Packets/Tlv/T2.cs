using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T2 : TlvBase
    {
        private readonly string _captchaCode;
        private readonly byte[] _captchaKey;

        public T2(string captchaCode, byte[] captchaKey)
        {
            _captchaCode = captchaCode;
            _captchaKey = captchaKey;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0002);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(0); // _sigVer
            PutString(_captchaCode, 2);
            PutBytes(_captchaKey, 2);
        }
    }
}

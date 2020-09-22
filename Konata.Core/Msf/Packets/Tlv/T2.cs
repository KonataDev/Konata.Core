using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T2 : TlvBase
    {
        public T2(string captchaCode, byte[] captchaKey)
            : base(0x0002, new T2Body(captchaCode, captchaKey))
        {

        }
    }

    public class T2Body : TlvBody
    {
        public readonly ushort _sigVer;
        public readonly string _captchaCode;
        public readonly byte[] _captchaKey;

        public T2Body(string captchaCode, byte[] captchaKey)
            : base()
        {
            _sigVer = 0;
            _captchaCode = captchaCode;
            _captchaKey = captchaKey;

            PutUshortBE(_sigVer);
            PutString(_captchaCode, 2);
            PutBytes(_captchaKey, 2);
        }

        public T2Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _sigVer);
            TakeString(out _captchaCode, Prefix.Uint16);
            TakeBytes(out _captchaKey, Prefix.Uint16);
        }
    }
}

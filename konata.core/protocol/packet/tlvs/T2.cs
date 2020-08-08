using System;
using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
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

        public override ushort GetTlvCmd()
        {
            return 0x02;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt16(0); // _sigVer
            builder.PushString(_captchaCode);
            builder.PushBytes(_captchaKey, false, true);
            return builder.GetPlainBytes();
        }

    }
}

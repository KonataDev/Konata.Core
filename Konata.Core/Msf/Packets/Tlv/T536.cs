using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T536 : TlvBase
    {
        public T536(byte[] loginExtraData)
            : base(0x0536, new T536Body(loginExtraData, loginExtraData.Length))
        {

        }
    }

    public class T536Body : TlvBody
    {
        public readonly byte[] _loginExtraData;

        public T536Body(byte[] loginExtraData, int loginExtraDataLength)
            : base()
        {
            _loginExtraData = loginExtraData;

            PutBytes(_loginExtraData);
        }

        public T536Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _loginExtraData, Prefix.Uint16);
        }
    }
}

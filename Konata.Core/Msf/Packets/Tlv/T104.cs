using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T104 : TlvBase
    {
        public T104(string sigSession)
            : base(0x0104, new T104Body(sigSession))
        {

        }
    }

    public class T104Body : TlvBody
    {
        public readonly string _sigSession;

        public T104Body(string sigSession)
            : base()
        {
            PutString(sigSession);
        }

        public T104Body(byte[] data)
            : base(data)
        {
            TakeString(out _sigSession, Prefix.None);
        }
    }
}

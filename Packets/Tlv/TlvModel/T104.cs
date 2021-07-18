using System;

namespace Konata.Core.Packets.Tlv.TlvModel
{
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

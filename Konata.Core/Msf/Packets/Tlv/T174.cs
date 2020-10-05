using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T174Body : TlvBody
    {
        public readonly byte[] _sigSecret;

        public T174Body(byte[] sigSecret, object nil)
            : base()
        {
            _sigSecret = sigSecret;

            PutBytes(_sigSecret);
        }

        public T174Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _sigSecret, Prefix.None);
        }
    }
}

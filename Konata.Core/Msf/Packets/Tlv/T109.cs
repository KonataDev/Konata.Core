using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T109Body : TlvBody
    {
        public readonly string _osType;

        public T109Body(string osType)
            : base()
        {
            _osType = osType;

            PutString(_osType);
        }

        public T109Body(byte[] data)
            : base(data)
        {
            TakeString(out _osType, Prefix.None);
        }
    }
}

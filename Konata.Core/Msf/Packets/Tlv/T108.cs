using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T108 : TlvBase
    {
        public T108(byte[] ksid)
            : base(0x0108, new T108Body(ksid, ksid.Length))
        {

        }
    }

    public class T108Body : TlvBody
    {
        public readonly byte[] _ksid;

        public T108Body(byte[] ksid, int ksidLength)
            : base()
        {
            _ksid = ksid;

            PutBytes(_ksid);
        }

        public T108Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _ksid, Prefix.None);
        }
    }
}

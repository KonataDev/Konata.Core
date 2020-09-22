using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T516 : TlvBase
    {
        public T516(uint sourceType = 0)
            : base(0x0516, new T516Body(sourceType))
        {

        }
    }

    public class T516Body : TlvBody
    {
        public readonly uint _sourceType;

        public T516Body(uint sourceType)
            : base()
        {
            _sourceType = sourceType;

            PutUintBE(_sourceType);
        }

        public T516Body(byte[] data)
            : base(data)
        {
            TakeUintBE(out _sourceType);
        }
    }
}

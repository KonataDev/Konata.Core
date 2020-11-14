using System;

namespace Konata.Packets.Tlv
{
    // 與T198相同?

    public class T197Body : TlvBody
    {
        public readonly byte _devLockMobileType;

        public T197Body(byte devLockMobileType)
            : base()
        {
            _devLockMobileType = devLockMobileType;

            PutByte(_devLockMobileType);
        }

        public T197Body(byte[] data)
            : base(data)
        {
            TakeByte(out _devLockMobileType);
        }
    }
}

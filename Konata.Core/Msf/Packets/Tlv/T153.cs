using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T153 : TlvBase
    {
        public T153(bool isRooted)
            : base(0x0153, new T153Body(isRooted))
        {

        }
    }

    public class T153Body : TlvBody
    {
        public readonly bool _isRooted;

        public T153Body(bool isRooted)
            : base()
        {
            _isRooted = isRooted;

            PutBoolBE(_isRooted, 2);
        }

        public T153Body(byte[] data)
            : base(data)
        {
            TakeBoolBE(out _isRooted, 2);
        }
    }
}

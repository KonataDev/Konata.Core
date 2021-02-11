using System;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T166Body : TlvBody
    {
        public readonly byte _imgType;

        public T166Body(byte imgType)
            : base()
        {
            _imgType = imgType;

            PutByte(_imgType);
        }

        public T166Body(byte[] data)
            : base(data)
        {
            TakeByte(out _imgType);
        }
    }
}

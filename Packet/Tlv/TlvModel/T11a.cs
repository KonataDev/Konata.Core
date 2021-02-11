using System;

namespace Konata.Core.Packet.Tlv.TlvModel
{
    public class T11aBody : TlvBody
    {
        public readonly ushort _face;
        public readonly byte _age;
        public readonly byte _gender;
        public readonly string _nickName;

        public T11aBody(ushort face, byte age, string nickName)
            : base()
        {
            _face = face;
            _age = age;
            _nickName = nickName;

            PutUshortBE(_face);
            PutByte(_age);
            PutString(_nickName, Prefix.Uint8);
        }

        public T11aBody(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _face);
            TakeByte(out _age);
            TakeByte(out _gender);
            TakeString(out _nickName, Prefix.Uint8);
        }
    }
}

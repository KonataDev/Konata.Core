using System;

namespace Konata.Packets.Tlv
{
    public class T17eBody : TlvBody
    {
        public readonly string _message;

        public T17eBody(string message)
            : base()
        {
            _message = message;

            PutString(_message);
        }

        public T17eBody(byte[] data)
            : base(data)
        {
            TakeString(out _message, Prefix.None);
        }
    }
}

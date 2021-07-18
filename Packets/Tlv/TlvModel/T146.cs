using System;

namespace Konata.Core.Packets.Tlv.TlvModel
{
    public class T146Body : TlvBody
    {
        public readonly ushort _ver;
        public readonly ushort _code;
        public readonly string _title;
        public readonly string _message;

        public readonly ushort _errorType;
        public readonly string _errorInfo;

        public T146Body(ushort code, string title, string message,
            ushort errorType, string errorInfo)
            : base()
        {
            _ver = 0;
            _code = code;
            _title = title;
            _message = message;
            _errorType = errorType;
            _errorInfo = errorInfo;

            PutUshortBE(_ver);
            PutUshortBE(_code);
            PutString(_title, Prefix.Uint16);
            PutString(_message, Prefix.Uint16);
            PutUshortBE(_errorType);
            PutString(_errorInfo, Prefix.Uint16);
        }

        public T146Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _ver);
            TakeUshortBE(out _code);
            TakeString(out _title, Prefix.Uint16);
            TakeString(out _message, Prefix.Uint16);
            TakeUshortBE(out _errorType);
            TakeString(out _errorInfo, Prefix.Uint16);
        }
    }
}

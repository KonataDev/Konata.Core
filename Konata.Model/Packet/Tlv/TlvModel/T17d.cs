using System;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T17dBody : TlvBody
    {
        public readonly ushort _mbGuideType;
        public readonly string _mbGuideMessage;

        public readonly ushort _mbGuideInfoType;
        public readonly string _mbGuideInfoMessage;

        public T17dBody(ushort mbGuideType, string mbGuideMessage,
            ushort mbGuideInfoType, string mbGuideInfoMessage)
            : base()
        {
            _mbGuideType = mbGuideType;
            _mbGuideMessage = mbGuideMessage;
            _mbGuideInfoType = mbGuideInfoType;
            _mbGuideInfoMessage = mbGuideInfoMessage;

            PutUshortBE(_mbGuideType);
            PutString(_mbGuideMessage, Prefix.Uint16);
            PutUshortBE(_mbGuideInfoType);
            PutString(_mbGuideInfoMessage, Prefix.Uint16);
        }

        public T17dBody(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _mbGuideType);
            TakeString(out _mbGuideMessage, Prefix.Uint16);
            TakeUshortBE(out _mbGuideInfoType);
            TakeString(out _mbGuideInfoMessage, Prefix.Uint16);
        }
    }
}

using System;
using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T8 : TlvBase
    {
        private readonly int _localId;
        private readonly short _timeZoneVer;
        private readonly short _timeZoneOffset;

        public T8(int localId = 2052, short timeZoneVer = 0, short timeZoneOffset = 0)
        {
            _localId = localId;
            _timeZoneVer = timeZoneVer;
            _timeZoneOffset = timeZoneOffset;
        }

        public override ushort GetTlvCmd()
        {
            return 0x08;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt16(_timeZoneVer);
            builder.PushInt32(_localId);
            builder.PushInt16(_timeZoneOffset);
            return builder.GetPlainBytes();
        }

    }
}
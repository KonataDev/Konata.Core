using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T8 : TlvBase
    {
        public T8(int localId = 2052, short timeZoneVer = 0, short timeZoneOffset = 0)
            : base(0x0008, new T8Body(localId, timeZoneVer, timeZoneOffset))
        {

        }
    }

    public class T8Body : TlvBody
    {
        public readonly int _localId;
        public readonly short _timeZoneVer;
        public readonly short _timeZoneOffset;

        public T8Body(int localId, short timeZoneVer, short timeZoneOffset)
            : base()
        {
            _localId = localId;
            _timeZoneVer = timeZoneVer;
            _timeZoneOffset = timeZoneOffset;

            PutShortBE(_timeZoneVer);
            PutIntBE(_localId);
            PutShortBE(_timeZoneOffset);
        }

        public T8Body(byte[] data)
            : base(data)
        {
            TakeShortBE(out _timeZoneVer);
            TakeIntBE(out _localId);
            TakeShortBE(out _timeZoneOffset);
        }
    }
}

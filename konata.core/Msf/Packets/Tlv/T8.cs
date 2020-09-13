using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
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

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0008);
        }

        public override void PutTlvBody()
        {
            PutShortBE(_timeZoneVer);
            PutIntBE(_localId);
            PutShortBE(_timeZoneOffset);
        }
    }
}

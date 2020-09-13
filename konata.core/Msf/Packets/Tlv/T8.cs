using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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
            return 0x08;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutShortBE(_timeZoneVer);
            builder.PutIntBE(_localId);
            builder.PutShortBE(_timeZoneOffset);
            return builder.GetBytes();
        }

    }
}
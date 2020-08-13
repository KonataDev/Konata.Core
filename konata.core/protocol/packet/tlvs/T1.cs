using System;
using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T1 : TlvBase
    {
        private readonly long _uin;
        private readonly byte[] _ipAddress;

        public T1(long uin, byte[] ipAddress)
        {
            _uin = uin;
            _ipAddress = ipAddress;
        }

        public override ushort GetTlvCmd()
        {
            return 0x01;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt16(1); // _ip_ver
            builder.PushInt32(new Random().Next());
            builder.PushInt32((int)_uin);
            builder.PushUInt32((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PushBytes(_ipAddress);
            builder.PushInt16(0);
            return builder.GetBytes();
        }
    }
}

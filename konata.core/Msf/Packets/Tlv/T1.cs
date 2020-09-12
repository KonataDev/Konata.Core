using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T1 : TlvBase
    {
        private readonly uint _uin;
        private readonly byte[] _ipAddress;

        public T1(uint uin, byte[] ipAddress)
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
            builder.PutUshortBE(1); // _ip_ver
            builder.PutIntBE(new Random().Next());
            builder.PutUintBE(_uin);
            builder.PutUintBE((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.PutBytes(_ipAddress);
            builder.PutUshortBE(0);
            return builder.GetBytes();
        }
    }
}

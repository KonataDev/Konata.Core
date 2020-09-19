using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T1 : TlvBase
    {
        private readonly uint _uin;
        private readonly byte[] _ipAddress;

        public T1(uint uin, byte[] ipAddress)
        {
            _uin = uin;
            _ipAddress = ipAddress;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0001);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(1); // _ip_ver
            PutIntBE(new Random().Next());
            PutUintBE(_uin);
            PutUintBE((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            PutBytes(_ipAddress);
            PutUshortBE(0);
        }
    }
}

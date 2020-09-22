using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T1 : TlvBase
    {
        public T1(uint uin, byte[] ipAddress)
            : base(0x0001, new T1Body(uin, ipAddress))
        {

        }

        public T1(byte[] data)
            : base(data)
        {
            _tlvBody = new T1Body(TakeAllBytes(out byte[] _));
        }
    }

    public class T1Body : TlvBody
    {
        private readonly uint _uin;
        private readonly ushort _ipVersion;
        private readonly byte[] _ipAddress;
        private readonly int _randomNumber;
        private readonly uint _timeNow;

        public T1Body(uint uin, byte[] ipAddress)
            : base()
        {
            _uin = uin;
            _ipAddress = ipAddress;
            _ipVersion = 1;
            _randomNumber = new Random().Next();
            _timeNow = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            PutUshortBE(_ipVersion);
            PutIntBE(_randomNumber);
            PutUintBE(_uin);
            PutUintBE(_timeNow);
            PutBytes(_ipAddress);
            PutUshortBE(0);
        }

        public T1Body(byte[] data)
            : base(data)
        {
            TakeUshortBE(out _ipVersion);
            TakeIntBE(out _randomNumber);
            TakeUintBE(out _uin);
            TakeUintBE(out _timeNow);
            TakeBytes(out _ipAddress, 4);
            EatBytes(2);
        }
    }
}

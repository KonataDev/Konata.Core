using System;
using System.IO;

namespace Konata.Core.Packets.Tlv.TlvModel
{
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

        internal T1Body(byte[] data)
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

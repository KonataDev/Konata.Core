using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T178Body : TlvBody
    {
        public readonly string _phone;
        public readonly string _countryCode;
        public readonly uint _smsCodeStatus;
        public readonly ushort _smsAvailableCount;
        public readonly ushort _smsTimeLimit;

        public T178Body(string phone, string countryCode, uint smsCodeStatus,
            ushort smsAvailableCount, ushort smsTimeLimit)
            : base()
        {
            _phone = phone;
            _countryCode = countryCode;
            _smsCodeStatus = smsCodeStatus;
            _smsAvailableCount = smsAvailableCount;
            _smsTimeLimit = smsTimeLimit;

            PutString(_countryCode, Prefix.Uint16);
            PutString(_phone, Prefix.Uint16);
            PutUintBE(_smsCodeStatus);
            PutUshortBE(_smsAvailableCount);
            PutUshortBE(_smsTimeLimit);
        }

        public T178Body(byte[] data)
            : base(data)
        {
            TakeString(out _countryCode, Prefix.Uint16);
            TakeString(out _phone, Prefix.Uint16);
            TakeUintBE(out _smsCodeStatus);
            TakeUshortBE(out _smsAvailableCount);
            TakeUshortBE(out _smsTimeLimit);
        }
    }
}

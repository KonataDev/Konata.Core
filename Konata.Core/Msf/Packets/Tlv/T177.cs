using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T177Body : TlvBody
    {
        public readonly uint _buildTime;
        public readonly string _sdkVersion;

        public T177Body(uint buildTime, string sdkVersion)
            : base()
        {
            _buildTime = buildTime;
            _sdkVersion = sdkVersion;

            PutByte(1);
            PutUintBE(_buildTime);
            PutString(_sdkVersion, Prefix.Uint16);
        }

        public T177Body(byte[] data)
            : base(data)
        {
            EatBytes(1);
            TakeUintBE(out _buildTime);
            TakeString(out _sdkVersion, Prefix.Uint16);
        }
    }
}

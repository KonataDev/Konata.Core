using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T177 : TlvBase
    {
        private readonly uint _buildTime;
        private readonly string _sdkVersion;

        public T177(uint buildTime, string sdkVersion)
        {
            _buildTime = buildTime;
            _sdkVersion = sdkVersion;
        }

        public override ushort GetTlvCmd()
        {
            return 0x177;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutByte(1);
            builder.PutUintBE(_buildTime);
            builder.PutString(_sdkVersion);
            return builder.GetBytes();
        }
    }
}

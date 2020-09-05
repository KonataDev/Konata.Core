using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T177 : TlvBase
    {
        private readonly long _buildTime;
        private readonly string _sdkVersion;

        public T177(long buildTime, string sdkVersion)
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
            builder.PushInt8(1);
            builder.PushInt32((int)_buildTime);
            builder.PushString(_sdkVersion);
            return builder.GetBytes();
        }
    }
}

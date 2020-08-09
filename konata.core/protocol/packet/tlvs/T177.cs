using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T177 : TlvBase
    {
        private readonly long _buildTime;
        private readonly string _sdkVersion;

        public T177(long buildTime = 1577331209, string sdkVersion = "6.0.0.2425")
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
            return builder.GetPlainBytes();
        }
    }
}
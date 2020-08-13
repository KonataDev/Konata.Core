using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T16e : TlvBase
    {
        private readonly string _deviceName;

        public T16e(string deviceName)
        {
            _deviceName = deviceName;
        }

        public override ushort GetTlvCmd()
        {
            return 0x16e;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushString(_deviceName, false);
            return builder.GetBytes();
        }
    }
}

using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T52d : TlvBase
    {
        private readonly byte[] _deviceReportInfo;

        public T52d(byte[] deviceReportInfo)
        {
            _deviceReportInfo = deviceReportInfo;
        }

        public override ushort GetTlvCmd()
        {
            return 0x52d;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(_deviceReportInfo, false);
            return builder.GetPlainBytes();
        }
    }
}
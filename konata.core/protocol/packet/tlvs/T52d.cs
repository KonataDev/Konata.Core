using System.IO;
using System.Text;
using ProtoBuf;
using Konata.Utils;
using Konata.Protocol.Protobuf;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T52d : TlvBase
    {
        private readonly byte[] _deviceReportInfo;

        public T52d(byte[] deviceReportInfo)
        {
            _deviceReportInfo = deviceReportInfo;
        }

        public T52d(string bootLoader, string version, string codeName, string incremental,
            string fingerprint, string bootId, string androidId, string baseBand, string innerVersion)
        {
            DeviceReport report = new DeviceReport
            {
                Bootloader = Encoding.UTF8.GetBytes(bootLoader),
                Version = Encoding.UTF8.GetBytes(version),
                CodeName = Encoding.UTF8.GetBytes(codeName),
                Incremental = Encoding.UTF8.GetBytes(incremental),
                Fingerprint = Encoding.UTF8.GetBytes(fingerprint),
                BootId = Encoding.UTF8.GetBytes(bootId),
                AndroidId = Encoding.UTF8.GetBytes(androidId),
                BaseBand = Encoding.UTF8.GetBytes(baseBand),
                InnerVersion = Encoding.UTF8.GetBytes(innerVersion)
            };

            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, report);

            _deviceReportInfo = stream.ToArray();
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
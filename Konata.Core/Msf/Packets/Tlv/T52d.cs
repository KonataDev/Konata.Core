using System.IO;
using System.Text;
using Konata.Msf.Packets.Protobuf;
using ProtoBuf;

namespace Konata.Msf.Packets.Tlv
{
    public class T52d : TlvBase
    {
        public T52d(byte[] deviceReportInfo)
            : base(0x052d, new T52dBody(deviceReportInfo, deviceReportInfo.Length))
        {

        }

        public T52d(string bootLoader, string version, string codeName,
            string incremental, string fingerprint, string bootId, string androidId,
            string baseBand, string innerVersion)

            : base(0x052d, new T52dBody(bootLoader, version, codeName,
             incremental, fingerprint, bootId, androidId, baseBand, innerVersion))
        {

        }
    }

    public class T52dBody : TlvBody
    {
        public readonly byte[] _deviceReportInfo;

        public T52dBody(byte[] deviceReportInfo, int deviceReportInfoLength)
            : base()
        {
            _deviceReportInfo = deviceReportInfo;

            PutBytes(_deviceReportInfo);
        }

        public T52dBody(string bootLoader, string version, string codeName,
            string incremental, string fingerprint, string bootId, string androidId,
            string baseBand, string innerVersion)
            : base()
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

            var stream = new MemoryStream();
            Serializer.Serialize(stream, report);

            _deviceReportInfo = stream.ToArray();

            PutBytes(_deviceReportInfo);
        }

        public T52dBody(byte[] data)
            : base(data)
        {
            TakeBytes(out _deviceReportInfo, Prefix.None);
        }
    }
}

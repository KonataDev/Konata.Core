using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    public class DeviceReport : ProtoTreeRoot
    {
        public DeviceReport(string bootLoader, string version,
            string codeName, string incremental, string fingerprint,
            string bootId, string androidId, string baseBand, string innerVersion)
        {
            addLeafString("0A", bootLoader);
            addLeafString("12", version);
            addLeafString("1A", codeName);
            addLeafString("22", incremental);
            addLeafString("2A", fingerprint);
            addLeafString("32", bootId);
            addLeafString("3A", androidId);
            addLeafString("42", baseBand);
            addLeafString("4A", innerVersion);
        }
    }
}

using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    public class DeviceReport : ProtoNode
    {
        public DeviceReport(string bootLoader, string version,
            string codeName, string incremental, string fingerprint,
            string bootId, string androidId, string baseBand, string innerVersion)
        {
            addLeaf("0A", bootLoader);
            addLeaf("12", version);
            addLeaf("1A", codeName);
            addLeaf("22", incremental);
            addLeaf("2A", fingerprint);
            addLeaf("32", bootId);
            addLeaf("3A", androidId);
            addLeaf("42", baseBand);
            addLeaf("4A", innerVersion);
        }
    }
}

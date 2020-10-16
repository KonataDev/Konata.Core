using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    public class DeviceReport : ProtoNode
    {
        public DeviceReport(string bootLoader, string version,
            string codeName, string incremental, string fingerprint,
            string bootId, string androidId, string baseBand, string innerVersion)
        {
            addTreeLeaf("0A", bootLoader);
            addTreeLeaf("12", version);
            addTreeLeaf("1A", codeName);
            addTreeLeaf("22", incremental);
            addTreeLeaf("2A", fingerprint);
            addTreeLeaf("32", bootId);
            addTreeLeaf("3A", androidId);
            addTreeLeaf("42", baseBand);
            addTreeLeaf("4A", innerVersion);
        }
    }
}

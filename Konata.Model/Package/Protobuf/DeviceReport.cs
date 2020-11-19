using Konata.Utils.Protobuf;

namespace Konata.Model.Package.Protobuf
{
    public class DeviceReport : ProtoTreeRoot
    {
        public DeviceReport(string bootLoader, string version,
            string codeName, string incremental, string fingerprint,
            string bootId, string androidId, string baseBand, string innerVersion)
        {
            AddLeafString("0A", bootLoader);
            AddLeafString("12", version);
            AddLeafString("1A", codeName);
            AddLeafString("22", incremental);
            AddLeafString("2A", fingerprint);
            AddLeafString("32", bootId);
            AddLeafString("3A", androidId);
            AddLeafString("42", baseBand);
            AddLeafString("4A", innerVersion);
        }
    }
}

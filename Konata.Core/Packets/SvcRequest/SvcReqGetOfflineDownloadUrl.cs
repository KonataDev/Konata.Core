using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.SvcRequest;

internal class SvcReqGetOfflineDownloadUrl: ProtoTreeRoot
{
    internal SvcReqGetOfflineDownloadUrl(int sequence, uint selfUin, string fileUuid)
    {
        AddLeafVar("08", 1200); // Cmd
        AddLeafVar("10", sequence); // Sequence
            
        AddTree("72", leaf =>
        {
            leaf.AddLeafVar("50", selfUin);
            leaf.AddLeafString("A201", fileUuid);
            leaf.AddLeafVar("F001", 2); // Owner tyoe
        }); // ApplyDownloadReq

        AddLeafVar("A806", 3); // BusinessId
        AddLeafVar("B006", 104); // Client type

        AddTree("FAE930", leaf =>
        {
            leaf.AddLeafVar("C0852C", 1); // DownloadUrlType
        }); // ExtensionReq
    }
}
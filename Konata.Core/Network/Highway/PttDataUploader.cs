using System.Threading.Tasks;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Network.Highway;

internal class PttDataUploader : HighwayUploader
{
    public enum DataType
    {
        GroupPtt,
        PrivatePtt
    };

    /// <summary>
    /// Ptt type
    /// </summary>
    public DataType PttType { get; set; }

    public uint DestUin { get; set; }

    public RecordChain PttInfo { get; set; }

    public async Task<bool> Upload()
    {
        // Set command id
        Command = PttType == DataType.GroupPtt
            ? PicUp.CommandId.GroupPttDataUp
            : PicUp.CommandId.FriendPttDataUp;

        var task = SendRequest(
            PttInfo.PttUpInfo.Host,
            PttInfo.PttUpInfo.Port,
            PttInfo.PttUpInfo.UploadTicket,
            PttInfo.FileData,
            PttType == DataType.GroupPtt
                ? new GroupPttUpRequest(AppInfo, DestUin, SelfUin, PttInfo)
                : new FriendPttUpRequest(DestUin, SelfUin, PttInfo)
        );

        // Wait for task
        var results = await task;
        {
            // Assert result is ok
            var code = results.PathTo<ProtoVarInt>("18");
            if (code != 0) return false;

            if (PttType == DataType.GroupPtt)
            {
                // Group ptt id and token
                var uploadInfo = results.PathTo<ProtoTreeRoot>("3A.2A");
                PttInfo.PttUpInfo.FileKey = uploadInfo.GetLeafString("5A");
                PttInfo.PttUpInfo.UploadId = (uint) uploadInfo.GetLeafVar("40");
            }

            else
            {
                // Friend ptt id and token
                var uploadInfo = results.PathTo<ProtoTreeRoot>("3A.3A");
                PttInfo.PttUpInfo.FileKey = uploadInfo.GetLeafString("D205");
                PttInfo.PttUpInfo.UploadId = uploadInfo.GetLeafBytes("A206");
            }

            return true;
        }
    }
}

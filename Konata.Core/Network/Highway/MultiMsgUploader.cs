using System.Threading.Tasks;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Network.Highway;

internal class MultiMsgUploader : HighwayUploader
{
    public uint DestUin { get; set; }
    
    public MultiMsgChain MultiMessages { get; set; }

    public MultiMsgUploader()
        => Command = PicUp.CommandId.MultiMsgDataUp;
    
    public async Task<bool> Upload()
    {
        var data = new MultiMsgUpRequest(DestUin,
            MultiMessages.PackedData, MultiMessages.MultiMsgUpInfo.MsgUKey);
        
        var task = SendRequest(
            MultiMessages.MultiMsgUpInfo.Host,
            MultiMessages.MultiMsgUpInfo.Port,
            MultiMessages.MultiMsgUpInfo.UploadTicket,
            ProtoTreeRoot.Serialize(data).GetBytes()
        );

        // Wait for task
        var results = await task;
        {
            var result = results.GetLeafVar("18");
            if (result != 0) return false;

            return true;
        }
    }
}

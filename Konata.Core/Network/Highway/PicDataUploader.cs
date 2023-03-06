using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;

namespace Konata.Core.Network.Highway;

internal class PicDataUploader : HighwayUploader
{
    public enum DataType
    {
        GroupImage,
        PrivateImage
    };

    /// <summary>
    /// Image type
    /// </summary>
    public DataType ImageType { get; set; }

    /// <summary>
    /// Image list
    /// </summary>
    public IEnumerable<ImageChain> Images { get; set; }

    public async Task<bool> Upload()
    {
        // Set command id
        Command = ImageType == DataType.GroupImage
            ? PicUp.CommandId.GroupPicDataUp
            : PicUp.CommandId.FriendPicDataUp;

        // Queue all tasks
        var tasks = new List<Task<HwResponse>>();
        foreach (var i in Images)
        {
            if (!i.PicUpInfo.UseCached)
            {
                tasks.Add(SendRequest(
                    i.PicUpInfo.Host,
                    i.PicUpInfo.Port,
                    i.PicUpInfo.UploadTicket,
                    i.FileData
                ));
            }
        }

        // Wait for tasks
        var results = await Task.WhenAll(tasks);
        return results.Count(i => i != null) == results.Length;
    }
}

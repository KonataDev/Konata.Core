using System.Threading.Tasks;
using Konata.Core.Common;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Packets.Protobuf.Highway.Requests;

namespace Konata.Core.Network.Highway;

internal class ImageOcrUploader : HighwayUploader
{
    public ServerInfo Server { get; set; }

    public byte[] UploadTicket { get; set; }

    public byte[] ImageData { get; set; }

    public string ImageGuid { get; set; }

    public ImageOcrUploader()
        => Command = PicUp.CommandId.ImageOcrDataUp;

    public async Task<string> Upload()
    {
        // Wait for tasks
        var result = await SendRequest(
            Server.Host,
            Server.Port,
            UploadTicket,
            ImageData,
            new ImageOcrUpRequest(ImageGuid)
        );

        // Check result code
        var code = result.GetLeafVar("18");
        if (code != 0) return null;

        return result.GetTree("3A").GetLeafString("12");
    }
}

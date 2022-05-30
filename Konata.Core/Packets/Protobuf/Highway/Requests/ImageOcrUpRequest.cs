using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway.Requests;

internal class ImageOcrUpRequest : ProtoTreeRoot
{
    public ImageOcrUpRequest(string guid)
    {
        AddLeafVar("08", 0);
        AddLeafString("12", guid);
    }
}

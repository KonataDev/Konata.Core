namespace Konata.Core.Packets.Oidb.Model;

internal class OidbCmd0xe07_0 : OidbCmd0xe07
{
    public OidbCmd0xe07_0(string imageUrl, uint imageLen,
        uint imageWidth, uint imageHeight, string md5) : base(0, new ReqBody
    {
        version = 1,
        client = 0,
        entrance = 1,
        ocrReqBody = new()
        {
            imageUrl = imageUrl,
            originMd5 = md5,
            afterCompressMd5 = md5,
            afterCompressFileSize = imageLen,
            afterCompressWeight = imageWidth,
            afterCompressHeight = imageHeight,
            isCut = false
        }
    })
    {
    }
}

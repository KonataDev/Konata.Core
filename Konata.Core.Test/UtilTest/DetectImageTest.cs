using System.Net.Http;
using System.Threading.Tasks;
using Konata.Core.Utils.FileFormat;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class DetectImageTest
{
    [Test]
    public async Task TestDetectImage()
    {
        var c = new HttpClient();
        var bs = await c.GetByteArrayAsync("https://cdn.mo2.leezeeyee.com/6" +
            "03ba0e2dfacf44803d8c780/1659593390232351509image.png~thumb");
        var r = FileFormat.DetectImage(bs, out var type, out var w, out var h);
        Assert.AreEqual(type, FileFormat.ImageFormat.Webp);
        Assert.AreEqual(w, 304);
        Assert.AreEqual(h, 131);
    }
}

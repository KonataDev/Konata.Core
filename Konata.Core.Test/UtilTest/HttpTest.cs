using System.Net.Http;
using System.Threading.Tasks;
using Konata.Core.Utils.Network;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class HttpTest
{
    [Test]
    public async Task TestHttpGet()
    {
        var http = await Http.Get("https://example.com");
        Assert.IsNotNull(http);
    }

    [Test]
    public async Task TestHttpPost()
    {
        var http = await Http.Post("https://example.com", new byte[] {0});
        Assert.IsNotNull(http);
    }

    [Test]
    public async Task TestHttpGetWithLimit()
    {
        // limit 100 Bytes
        try
        {
            await Http.Get("https://example.com", limitLen: 100);
        }
        catch (HttpRequestException e)
        {
            Assert.Pass(e.Message);
        }
    }
}

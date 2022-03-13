using Konata.Core.Message;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class ChainTest
{
    [Test]
    public void TestChainCombine()
    {
        var builder = new MessageBuilder();
        {
            builder.Text("Hello!");
            builder.Text("World!");
        }
        var chain = builder.Build();
        Assert.AreEqual(chain.Count, 1);
    }
}

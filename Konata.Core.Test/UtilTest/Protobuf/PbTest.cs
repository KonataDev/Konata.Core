using NUnit.Framework;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Extensions;

namespace Konata.Core.Test.UtilTest;

public class PbTest
{
    [Test]
    public void TestPbDecoder1()
    { 
        var buf = "109a0521a4703d0ad7a30a4032096b6b6b6b6b6c6c6c6c420d0a0331323318a4132a03010305420a108906ba3e046f6f6f6f".UnHex();
        var root = ProtoTreeRoot.Deserialize(buf, true);
        Assert.AreEqual(root.GetLeafVar("10"), 666);
        // Assert.AreEqual(root.GetLeafDouble("21"), 3.33);
        Assert.AreEqual(root.GetLeafString("32"), "kkkkkllll");
        // root.GetLeaves<ProtoTreeRoot>("42").ForEach((v) =>
        // {
        //     
        // });
    }
    
    [Test]
    public void TestPbDecoder2()
    { 
        var buf = "10 9a 05 21 a4 70 3d 0a d7 a3 0a 40 32 09 6b 6b 6b 6b 6b 6c 6c 6c 6c 42 0d 0a 03 31 32 33 18 a4 13 2a 03 01 03 05 4a 08 08 f8 06 12 03 39 39 39 4a 08 08 ab 04 12 03 6b 6b 6b".UnHex();
        var pb = ProtobufDecoder.Create(buf);
        Assert.AreEqual(pb[2].AsNumber(), 666);
        Assert.AreEqual(pb[6].AsString(), "kkkkkllll");
        Assert.AreEqual(pb[8][1].AsString(), "123");
        Assert.AreEqual(pb[8][3].AsNumber(), 2468);
        var list = pb[9].AsLeaves();
        Assert.AreEqual(list[0][1].AsNumber(), 888);
        Assert.AreEqual(list[0][2].AsString(), "999");
        Assert.AreEqual(list[1][1].AsNumber(), 555);
        Assert.AreEqual(list[1][2].AsString(), "kkk");
    }
}

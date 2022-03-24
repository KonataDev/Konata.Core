using System;
using System.Collections.Generic;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.Protobuf;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest.Protobuf;

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
        var bytes = "10 9a 05 21 a4 70 3d 0a d7 a3 0a 40 32 09 6b 6b 6b 6b 6b 6c 6c 6c 6c 42 0d 0a 03 31 32 33 18 a4 13 2a 03 01 03 05 4a 08 08 f8 06 12 03 39 39 39 4a 08 08 ab 04 12 03 6b 6b 6b".UnHex();
        var pb = ProtobufDecoder.Create(bytes);
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

    public byte[] TestPbEncoderNew()
    {
        var tree = new Proto.Tree
        {
            {1, 123},
            {2, "111"},
            {3, 5555555555555566},
            {4,  new Proto.Tree
            { 
                {5, "1"},
                {7, 555556666},
                {8, new Proto.List
                {
                    new Proto.Tree
                    {
                        {1, 3},
                        {2, 4}
                    },
                    new Proto.Tree
                    {
                        {2, "5"},
                        {6, 3333}
                    }
                }}
            }},
        };

        return Proto.Encode(tree);
    }

    public byte[] TestPbEncoderOld()
    {
        var tree2 = new ProtoTreeRoot();
        {
            tree2.AddLeafVar("08", 123);
            tree2.AddLeafString("12", "111");
            tree2.AddLeafVar("18", 5555555555555566);
            tree2.AddTree("22", _ =>
            {
                _.AddLeafString("2A", "1");
                _.AddLeafVar("38", 555556666);
                _.AddTree("42", __=>
                {
                    __.AddLeafVar("08", 3);
                    __.AddLeafVar("10", 4);
                });
                _.AddTree("42", __=>{
                    __.AddLeafString("12", "5");
                    __.AddLeafVar("30", 3333);
                });
            });
        }

        return ProtoTreeRoot.Serialize(tree2).GetBytes();
    }

    [Test]
    public void TestPbEncoder1()
        => Assert.AreEqual(TestPbEncoderNew(), TestPbEncoderOld());

    [Test]
    public void TestBenchNew()
    {
        for(var i = 0; i< 100000; ++i)
            TestPbEncoderNew();
    }

    [Test]
    public void TestBenchOld()
    {
        for(var i = 0; i< 100000; ++i)
            TestPbEncoderOld();
    }
}

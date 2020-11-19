using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Konata.Utils;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Konata.Test
{
    public enum testenum
    {
        test1=1,
        test2=2
    }
    public class TestModel
    {
        public string test { get; set; }
        private string ptest { get; set; }
        public int itest { get; set; }

        public float ftest { get; set; }
        public testenum enumtest { get; set; } = testenum.test2;
    }

    [TestFixture(Description ="工具类组件测试")]
    public class UtilsTest: BaseTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行工具类测试");
        }

        [Test]
        [Category("byte缓存字节测试")]
        public void ByteBuffer_Funcation()
        {
            var buffer = new ByteBuffer();
            buffer.PutBytes(new byte[0],
                                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            buffer.PutString("Test",
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);


            buffer.TakeUintBE(out var length);
            buffer.TakeString(out var outstr,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);


            Assert.AreEqual(length, 4);
            Assert.AreEqual(outstr, "Test");
        }

        [Test]
        [Category("proto树结构展示")]
        public void Proto_Tree_Funcation()
        {
            var root = new ProtoTreeRoot();
            var root1 = new ProtoTreeRoot();
            var root2 = new ProtoTreeRoot();
            var root3 = new ProtoTreeRoot();
            root3.AddLeafString("0A", "Hello Konata!");
            root2.AddTree("0A", root3);
            root1.AddTree("0A", root2);
            root.AddTree("0A", root1);

            Print_Bytes(root.Serialize().GetBytes());
            Assert.Pass();
        }

        [Test]
        [Category("配置文件装载")]
        public void Load_Config()
        {
            var config = ConfigurationReader.LoadConfig(basepath:Directory.GetCurrentDirectory(),reloadOnChange:true);
            Console.WriteLine(ConfigurationReader.CurrentPath);
            Console.WriteLine(Directory.GetCurrentDirectory());
            TestModel tm = new TestModel();
            //TestModel tm2 = new TestModel();
            config.Bind("testbind", tm);
            //config.Bind(tm2, bindOption=> { bindOption.BindNonPublicProperties = true;});

            Assert.AreEqual("test", config["test"]);
            Assert.AreEqual(testenum.test1, tm.enumtest);
        }
    }
}

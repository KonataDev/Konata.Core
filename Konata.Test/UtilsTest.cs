using System;
using System.IO;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

using Konata.Utils;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;

namespace Konata.Test
{
    public enum TestEnum
    {
        test1 = 1,
        test2 = 2
    }

    public class TestModel
    {
        public string test { get; set; }

        private string ptest { get; set; }

        public int itest { get; set; }

        public float ftest { get; set; }

        public TestEnum enumtest { get; set; } = TestEnum.test2;
    }

    [TestFixture(Description = "工具类组件测试")]
    public class UtilsTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行工具类测试");
        }

        [Test]
        [Category("byte缓存字节测试")]
        public void ByteBuffer_Function()
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
        public void Proto_Tree_Function()
        {
            var root = new ProtoTreeRoot();
            {
                root.AddTree("0A", (ProtoTreeRoot root1) =>
                {
                    root1.AddTree("0A", (ProtoTreeRoot root2) =>
                    {
                        root2.AddTree("0A", (ProtoTreeRoot root3) =>
                        {
                            root3.AddLeafString("0A", "Hello Konata!");
                        });
                    });
                });
            }

            Print_Bytes(root.Serialize().GetBytes());
            Assert.Pass();
        }

        [Test]
        [Category("配置文件装载")]
        public void Load_Config()
        {
            var config = ConfigurationReader.LoadConfig
                (basepath: Directory.GetCurrentDirectory(), reloadOnChange: true);

            Console.WriteLine(ConfigurationReader.CurrentPath);
            Console.WriteLine(Directory.GetCurrentDirectory());

            TestModel testModel = new TestModel();
            config.Bind("testbind", testModel);

            Assert.AreEqual("test", config["test"]);
            Assert.AreEqual(TestEnum.test1, testModel.enumtest);
        }
    }
}

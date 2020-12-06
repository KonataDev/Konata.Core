using System;
using NUnit.Framework;

namespace Konata.Test.Simple
{
    /// <summary>
    /// 测试用例标准模板参考
    /// </summary>
    [TestFixture(Description = "测试模板")]
    public class SimpleTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行功能测试");
        }

        [Test(Description ="test2")]
        [Category("反射获取方法参数")]
        public void Test2()
        {

        }

        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
        }
    }
}
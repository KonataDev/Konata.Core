using Konata.Model.Localization;
using NUnit.Framework;
using System;
using System.Threading;

namespace Konata.Test.Simple
{
    /// <summary>
    /// 测试用例标准模板参考
    /// </summary>
    [TestFixture(Description ="测试模板")]
    public class SimpleTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行功能测试");
        }

        [Test(Description ="test1")]
        [Category("test for test")]
        public void Test1()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Assert.AreEqual(Localization.TestString, "TestString");
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Assert.AreEqual(Localization.TestString, "测试字符串");
        }


        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
        }
    }
}
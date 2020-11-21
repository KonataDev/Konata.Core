using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Konata.Core.MQ;
using Konata.Core.Builder;
using Konata.Core.NetWork;
using Konata.Core.Extensions;

namespace Konata.Test
{
    public class CoreTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行功能测试");
        }

        [Test(Description = "SocketTest")]
        [Category("SockeT测试")]
        public void Socket_Init()
        {
            ISocket socket = new SocketBuilder()
                .SetCustomSocket(typeof(KonataSocket))
                .SocketConfig(config =>
                {
                    config.Ip = "127.0.0.1";
                    config.Port = 10001;
                    config.MinPackageLen = 4;
                    config.ProtocolType = System.Net.Sockets.ProtocolType.Tcp;
                    config.SocketType = System.Net.Sockets.SocketType.Stream;
                    config.Timeout = 5000;
                    config.BufferSize = 1024;
                    config.TotalBufferSize = 4096;
                })
                .SetServerCloseWatcher(() =>
                {
                    Console.WriteLine("socket已关闭");
                })
                .SetRecvLenCalcer((bufferlist) =>
                {
                    if (bufferlist.Count < 3)
                    {
                        return -1;
                    }
                    byte[] lenBytes = bufferlist.GetRange(0, 3).ToArray();

                    return BitConverter.ToInt16(lenBytes, 0);
                })
                .SetServerDataReceiver((ref byte[] bytes) =>
                {
                    Console.WriteLine("接收到了一个报文");
                })
                .Build();
        }


        [Test(Description = "MQTest")]
        [Category("消息队列测试")]
        public void MQ_Test()
        {
            IMQ<string> MQ = new MQBuilder<string>()
                .MQConfig(config =>
                {
                    config.MaxProcessMTask = 4;
                })
                .AddMQReceiver((data) =>
                {
                    Console.WriteLine($"received data {data}");
                })
                .Build();
            MQ.StartTakeProcess();

            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("A:Ready add data...");
                    MQ.Add($"A:this is {i} time added");
                }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("B:Ready add data...");
                    MQ.Add($"B:this is {i} time added");
                }
            });
        }

        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
        }
    }
}

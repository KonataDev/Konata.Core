using Konata.Core.Builder;
using Konata.Core.NetWork;
using Konata.Core.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
                .SetServerCloseWatcher(()=>
                {
                    Console.WriteLine("socket已关闭");
                })
                .SetRecvLenCalcer((bufferlist) => {
                    if (bufferlist.Count < 3)
                    {
                        return -1;
                    }
                    byte[] lenBytes = bufferlist.GetRange(0, 3).ToArray();

                    return BitConverter.ToInt16(lenBytes, 0);
                })
                .SetServerDataReceiver((bytes) => {
                    Console.WriteLine("接收到了一个报文");
                })
                .Build();
        }


        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
        }
    }
}

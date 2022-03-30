using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Konata.Core.Utils.Network.TcpClient;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class FuturedSocketTest
{
    [Test]
    public static async Task Test()
    {
        var socket = new FuturedSocket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        {
            if (await socket.Connect("example.com", 80))
            {
                // Send fake http request
                await socket.Send(Encoding.UTF8.GetBytes
                    ("GET / HTTP/1.1\r\nHost: example.com\r\n\r\n"));

                var buffer = new byte[1024];
                var read = await socket.Receive(buffer);

                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, read));

                socket.Dispose();
                Assert.AreNotEqual(read, 0);
            }
        }
    }
}

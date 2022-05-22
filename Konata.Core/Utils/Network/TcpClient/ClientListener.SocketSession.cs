using System;
using System.Net.Sockets;
using System.Threading;

namespace Konata.Core.Utils.Network.TcpClient
{
    internal abstract partial class ClientListener
    {
        protected sealed class SocketSession : IDisposable
        {
            public Socket Socket { get; }

            private CancellationTokenSource _cts;

            public CancellationToken Token { get; }

            public SocketSession()
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _cts = new CancellationTokenSource();
                Token = _cts.Token;
            }

            public void Dispose()
            {
                CancellationTokenSource cts = Interlocked.Exchange(ref _cts, null);
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    Socket.Dispose();
                }
            }
        }
    }
}

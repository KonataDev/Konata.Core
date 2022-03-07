using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PossibleNullReferenceException
// ReSharper disable RedundantAssignment
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable TooWideLocalVariableScope

namespace Konata.Core.Utils.TcpSocket
{
    internal class AsyncClient
    {
        /// <summary>
        /// Socket connected
        /// </summary>
        public bool Connected
            => _socketInstance?.Connected ?? false;

        private int _socketIp;
        private string _socketHost;
        private FeaturedSocket _socketInstance;

        private IClientListener _listener;
        private readonly MemoryStream _recvStream;
        private readonly byte[] _recvBuffer;
        private long _packetLen;

        /// <summary>
        /// Construct a tcp client
        /// </summary>
        public AsyncClient()
        {
            _recvStream = new(2048);
            _recvBuffer = new byte [2048];
        }

        /// <summary>
        /// Construct a tcp client with
        /// a custom listener
        /// </summary>
        /// <param name="listener"></param>
        public AsyncClient(IClientListener listener) : this()
        {
            _listener = listener;
        }

        /// <summary>
        /// Set client listener
        /// </summary>
        /// <param name="listener"></param>
        public void SetListener(IClientListener listener)
            => _listener = listener;

        /// <summary>
        /// Connect to the server
        /// <param name="host"></param>
        /// <param name="ip"></param>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect(string host, int ip)
        {
            // The client has been connected
            if (_socketInstance != null) return false;
            if (_socketInstance?.Connected ?? false) return true;
            {
                _socketIp = ip;
                _socketHost = host;
            }

            // Connect to server
            return await Reconnect();
        }

        /// <summary>
        /// Reconnect to the server
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Reconnect()
        {
            // Cleanup
            await Disconnect();

            // Create socket
            _socketInstance = new FeaturedSocket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            {
                _socketInstance.ReceiveTimeout = 100;
            }

            // Connect to the server
            static void callback(IAsyncResult result) => ((FeaturedSocket) result.AsyncState).EndConnect(result);
            if (!_socketInstance.TryConnect(_socketHost, _socketIp, callback, _socketInstance))
            {
                return false;
            }


            // Connected
            if (_socketInstance.Connected)
            {
                _socketInstance.BeginReceive(_recvBuffer, 0,
                    _recvBuffer.Length, SocketFlags.None, BeginReceive, null);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <returns></returns>
        public Task<bool> Disconnect()
        {
            // Not connected
            if (_socketInstance == null) return Task.FromResult(true);

            // Disconnect
            if (_socketInstance.Connected)
                _socketInstance.Disconnect(false);
            {
                // And clanup
                _recvStream.SetLength(0);
                _socketInstance.Dispose();
                _socketInstance = null;
            }

            _listener?.OnDisconnect();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task<bool> Send(ReadOnlyMemory<byte> buffer)
        {
            // Not connected
            if (_socketInstance == null) return Task.FromResult(false);
            if (!_socketInstance.Connected) return Task.FromResult(false);

            // Send the data
            _socketInstance.BeginSend(buffer.ToArray(), 0, buffer.Length, SocketFlags.None,
                r => ((FeaturedSocket) r.AsyncState).EndSend(r), _socketInstance).AsyncWaitHandle.WaitOne();

            return Task.FromResult(true);
        }

        /// <summary>
        /// Recv the data
        /// </summary>
        private void BeginReceive(IAsyncResult result)
        {
            var recvLen = 0;

            try
            {
                // Not connected
                if (!_socketInstance.Connected)
                {
                    Disconnect();
                    return;
                }

                // Receiving the data
                recvLen = _socketInstance.EndReceive(result);

                // Write to stream
                _recvStream.Write(_recvBuffer, 0, recvLen);
                {
                    if (recvLen == 0) goto Final;

                    // Dissect the packet length
                    DissectNext:
                    if (_packetLen == 0)
                    {
                        _packetLen = _listener.OnStreamDissect
                            (_recvStream.GetBuffer(), (uint) _recvStream.Length);
                    }

                    // Cut down one packet
                    if (_packetLen > 0 &&
                        _recvStream.Length >= _packetLen)
                    {
                        // Read data from stream
                        var packetBuf = new byte[_packetLen];
                        {
                            _recvStream.Seek(0, SeekOrigin.Begin);
                            _recvStream.Read(packetBuf, 0, (int) _packetLen);

                            // Move the remaining data ahead
                            var streamBuf = _recvStream.GetBuffer();
                            var streamLen = _recvStream.Length - _recvStream.Position;

                            if (streamLen > 0)
                            {
                                Array.Copy(streamBuf, _recvStream.Position, streamBuf, 0, streamLen);
                                {
                                    _recvStream.SetLength(streamLen);
                                    _recvStream.Seek(streamLen, SeekOrigin.Begin);
                                }
                            }
                            else _recvStream.SetLength(0);

                            // Reset
                            _packetLen = 0;
                        }

                        // Call on recv packet
                        _listener.OnRecvPacket(packetBuf);

                        // Dissect next packet
                        if (_recvStream.Length > 0) goto DissectNext;
                    }
                }

                Final:
                _socketInstance.BeginReceive(_recvBuffer, 0,
                    _recvBuffer.Length, SocketFlags.None, BeginReceive, null);
            }

            // Catch exceptions
            catch (Exception)
            {
                Disconnect();
            }
        }
    }

    class FeaturedSocket : Socket
    {
        public FeaturedSocket(AddressFamily addressFamily, SocketType socketType,
            ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }

        public bool TryConnect(string host, int port, AsyncCallback requestCallback, object state)
        {
            BeginConnect(host, port, requestCallback, state).AsyncWaitHandle.WaitOne();
            return this.Connected;
        }

        public new bool EndConnect(IAsyncResult result)
        {
            try
            {
                base.EndConnect(result);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

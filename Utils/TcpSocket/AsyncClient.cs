using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            => _socketInstance.Connected;

        private int _socketIp;
        private string _socketHost;
        private readonly Socket _socketInstance;

        private IClientListener _listener;
        private readonly MemoryStream _recvStream;
        private readonly byte[] _recvBuffer;

        /// <summary>
        /// Construct a tcp client
        /// </summary>
        public AsyncClient()
        {
            _recvStream = new(2048);
            _recvBuffer = new byte [2048];

            _socketInstance = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            {
                _socketInstance.ReceiveTimeout = 100;
            }
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
            if (_socketInstance.Connected) return true;
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
        public Task<bool> Reconnect()
        {
            // The client has been connected
            if (_socketInstance.Connected) return Task.FromResult(true);
            {
                // Connect to the server
                _socketInstance.BeginConnect(_socketHost, _socketIp,
                    r => ((Socket) r.AsyncState).EndConnect(r), _socketInstance).AsyncWaitHandle.WaitOne();

                // Connected
                if (_socketInstance.Connected)
                {
                    _socketInstance.BeginReceive(_recvBuffer, 0,
                        _recvBuffer.Length, SocketFlags.None, BeginReceive, null);

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <returns></returns>
        public Task<bool> Disconnect()
        {
            // Not connected
            if (!_socketInstance.Connected)
            {
                return Task.FromResult(false);
            }

            // Disconnect
            _socketInstance.Disconnect(true);
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
            if (!_socketInstance.Connected) return Task.FromResult(true);

            // Send the data
            _socketInstance.BeginSend(buffer.ToArray(), 0, buffer.Length, SocketFlags.None,
                r => ((Socket) r.AsyncState).EndSend(r), _socketInstance).AsyncWaitHandle.WaitOne();

            return Task.FromResult(true);
        }

        /// <summary>
        /// Recv the data
        /// </summary>
        private void BeginReceive(IAsyncResult result)
        {
            var recvLen = 0;
            var packetLen = 0U;

            // Not connected
            if (!_socketInstance.Connected) return;

            try
            {
                // Receiving the data
                recvLen = _socketInstance.EndReceive(result);

                // Write to stream
                _recvStream.Write(_recvBuffer, 0, recvLen);
                {
                    if (recvLen == 0) goto Final;

                    // Dissect the packet length
                    DissectNext:
                    if (packetLen == 0)
                    {
                        packetLen = _listener.OnStreamDissect
                            (_recvStream.GetBuffer(), (uint) _recvStream.Length);
                    }

                    // Cut down one packet
                    if (packetLen > 0 &&
                        _recvStream.Length >= packetLen)
                    {
                        // Read data from stream
                        var packetBuf = new byte[packetLen];
                        {
                            _recvStream.Seek(0, SeekOrigin.Begin);
                            _recvStream.Read(packetBuf, 0, (int) packetLen);

                            // Move the remaining data ahead
                            var streamBuf = _recvStream.GetBuffer();
                            var streamLen = _recvStream.Length - _recvStream.Position;
                            Array.Copy(streamBuf, _recvStream.Position, streamBuf, 0, streamLen);
                            {
                                _recvStream.SetLength(streamLen);
                                _recvStream.Seek(0, SeekOrigin.Begin);
                            }

                            // Reset
                            packetLen = 0;
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
            catch (Exception e)
            {
                if (_socketInstance.Connected)
                    _socketInstance.Disconnect(false);
            }
        }
    }
}

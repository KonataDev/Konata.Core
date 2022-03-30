using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PossibleNullReferenceException
// ReSharper disable RedundantAssignment
// ReSharper disable InvertIf
// ReSharper disable TooWideLocalVariableScope

namespace Konata.Core.Utils.Network.TcpClient;

internal class AsyncClient
{
    /// <summary>
    /// Socket connected
    /// </summary>
    public bool Connected
        => _socketInstance?.Connected ?? false;

    private string _socketHost;
    private ushort _socketPort;
    private FuturedSocket _socketInstance;

    private Thread _recvThread;
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
    /// <param name="port"></param>
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Connect(string host, int port)
    {
        // The client has been connected
        if (_socketInstance != null) return false;
        if (_socketInstance?.Connected ?? false) return true;
        {
            _socketHost = host;
            _socketPort = (ushort) port;
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

        try
        {
            // Create socket
            _socketInstance = new FuturedSocket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the server
            if (!await _socketInstance.Connect(_socketHost, _socketPort))
                return false;

            // Not connected
            if (!_socketInstance.Connected) return false;

            // Start receive the data
            _recvThread = new(BeginReceive);
            _recvThread.Start();
            return true;
        }
        catch (Exception e)
        {
            _listener?.OnSocketError(e);
            return false;
        }
    }

    /// <summary>
    /// Disconnect
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Disconnect()
    {
        try
        {
            // Not connected
            if (_socketInstance == null)
                return true;

            // Disconnect
            if (_socketInstance.Connected)
                await _socketInstance.Disconnect();

            // And clanup
            _packetLen = 0;
            _recvStream.SetLength(0);
            _socketInstance.Dispose();
            _socketInstance = null;

            _listener?.OnDisconnect();
            _recvThread.Join();
            return true;
        }
        catch (Exception e)
        {
            _listener?.OnSocketError(e);
            return false;
        }
    }

    /// <summary>
    /// Send data
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<bool> Send(byte[] buffer, int timeout = -1)
    {
        try
        {
            // Send the data
            if (_socketInstance is not {Connected: true}) return false;
            return await _socketInstance.Send(buffer, timeout) == buffer.Length;
        }
        catch (Exception e)
        {
            _listener?.OnSocketError(e);
            return false;
        }
    }

    /// <summary>
    /// Recv the data
    /// </summary>
    private async void BeginReceive()
    {
        var recvLen = 0;
        try
        {
            while (_socketInstance.Connected)
            {
                // Receiving the data
                recvLen = await _socketInstance.Receive(_recvBuffer);
                if (recvLen == 0) continue;

                // Write to stream
                _recvStream.Write(_recvBuffer, 0, recvLen);
                {
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
            }
        }

        // Catch exceptions
        catch (Exception e)
        {
            _listener.OnSocketError(e);
        }
    }
}

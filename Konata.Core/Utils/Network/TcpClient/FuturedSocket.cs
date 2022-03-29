using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Utils.Network.TcpClient;

internal class FuturedSocket : IDisposable
{
    /// <summary>
    /// Inner socket
    /// </summary>
    public Socket InnerSocket { get; }

    /// <summary>
    /// Is Connected
    /// </summary>
    public bool Connected
        => InnerSocket.Connected;

    public FuturedSocket(AddressFamily family, SocketType type, ProtocolType protocol)
        => InnerSocket = new(family, type, protocol);

    public void Dispose()
        => InnerSocket?.Dispose();

    /// <summary>
    /// Connect to server
    /// </summary>
    /// <param name="ep"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<bool> Connect(IPEndPoint ep, int timeout = -1)
    {
        EnterAsync(out var tk, out var args);
        {
            args.UserToken = tk;
            args.RemoteEndPoint = ep;
            args.Completed += OnCompleted;

            // Connect async
            if (InnerSocket.ConnectAsync(args))
                await Task.Run(() => tk.WaitOne(timeout));
        }
        LeaveAsync(tk, args);

        return InnerSocket.Connected;
    }

    /// <summary>
    /// Disconnect from server
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<bool> Disconnect(int timeout = -1)
    {
        EnterAsync(out var tk, out var args);
        {
            args.UserToken = tk;
            args.Completed += OnCompleted;

            // Disconnect async
            if (InnerSocket.DisconnectAsync(args))
                await Task.Run(() => tk.WaitOne(timeout));
        }
        LeaveAsync(tk, args);

        return InnerSocket.Connected == false;
    }

    /// <summary>
    /// Send data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<int> Send(byte[] data, int timeout = -1)
    {
        EnterAsync(out var tk, out var args);
        {
            args.UserToken = tk;
            args.Completed += OnCompleted;
            args.SetBuffer(data);

            // Send async
            if (InnerSocket.SendAsync(args))
                await Task.Run(() => tk.WaitOne(timeout));
        }
        LeaveAsync(tk, args);

        return args.BytesTransferred;
    }

    /// <summary>
    /// Receive data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<int> Receive(byte[] data, int timeout = -1)
    {
        EnterAsync(out var tk, out var args);
        {
            args.UserToken = tk;
            args.Completed += OnCompleted;
            args.SetBuffer(data);

            // Receive async
            if (InnerSocket.ReceiveAsync(args))
                await Task.Run(() => tk.WaitOne(timeout));
        }
        LeaveAsync(tk, args);

        return args.BytesTransferred;
    }

    #region Overload methods

    public async Task<bool> Connect(string host, int port)
    {
        // Try parse ipaddress
        if (IPAddress.TryParse(host, out var ipaddr))
            return await Connect(ipaddr, port);

        // Get ipaddress through Dns
        var ipList = await Dns.GetHostEntryAsync(host);
        if (ipList.AddressList.Length <= 0)
            throw new EntryPointNotFoundException("Dns probe returns no ip address.");

        // Connect it
        return await Connect(ipList.AddressList[0], port);
    }

    public Task<bool> Connect(IPAddress addr, int port)
        => Connect(new(addr, port));

    public Task<int> Send(string str)
        => Send(Encoding.UTF8.GetBytes(str));

    public Task<int> Send(ReadOnlyMemory<byte> bytes)
        => Send(bytes.ToArray());

    private static void OnCompleted(object s, SocketAsyncEventArgs e)
        => ((AutoResetEvent) e.UserToken)?.Set();

    private static void EnterAsync(out AutoResetEvent tk, out SocketAsyncEventArgs args)
    {
        tk = new AutoResetEvent(false);
        args = new SocketAsyncEventArgs();
    }

    private static void LeaveAsync(AutoResetEvent token, SocketAsyncEventArgs args)
    {
        args?.Dispose();
        token?.Dispose();
    }

    #endregion
}

using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Konata.Core.Common;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Protobuf;
using ClientListener = Konata.Core.Network.TcpClient.ClientListener;

namespace Konata.Core.Network.Highway;

internal class HighwayClient : ClientListener
{
    private readonly AppInfo _appInfo;
    private readonly uint _peer;
    private readonly byte[] _ticket;
    private int _sequence;
    private readonly Md5Cryptor _md5Cryptor;

    public override uint HeaderSize => 9;

    // We no need to use a dict to storage
    // the pending requests, cuz there's only
    // one request pending in the same time
    private HwResponse _hwResponse;
    private readonly ManualResetEvent _requestAwaiter;

    internal HighwayClient(AppInfo appInfo, uint peer, byte[] ticket)
    {
        _peer = peer;
        _ticket = ticket;
        _appInfo = appInfo;
        _sequence = new Random().Next(0x2333, 0x7090);
        _hwResponse = default;
        _requestAwaiter = new(false);
        _md5Cryptor = new();
    }

    /// <summary>
    /// Send Im coming
    /// </summary>
    /// <returns></returns>
    internal async Task<bool> Echo()
    {
        // Send request
        var result = await SendRequest
            (new PicUpEcho(_appInfo, _peer, _sequence));
        {
            // No response
            if (result == null) return false;

            // Assert checks
            if (result.Command != PicUpEcho.Command) return false;
            if (result.PeerUin != _peer) return false;

            return true;
        }
    }

    /// <summary>
    /// Data Up
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataMd5"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="cmdId"></param>
    /// <param name="extend"></param>
    /// <returns></returns>
    internal async Task<HwResponse> DataUp(byte[] data, byte[] dataMd5,
        int offset, int length, PicUp.CommandId cmdId, ProtoTreeRoot extend = null)
    {
        // Calculate chunk
        var chunk = data[offset..(offset + length)];
        var chunkMd5 = _md5Cryptor.Encrypt(chunk);

        // Send request
        var result = await SendRequest(new PicUpDataUp(cmdId, _appInfo, _peer, _sequence,
            _ticket, data.Length, dataMd5, offset, length, chunkMd5, extend), chunk);
        {
            // No response
            if (result == null) return null;

            // Assert checks
            if (result.Command != PicUpDataUp.Command) return null;
            if (result.PeerUin != _peer) return null;

            return result;
        }
    }

    private async Task<HwResponse> SendRequest
        (PicUp request, byte[] body = null)
    {
        // Send HwRequest
        await Send(HwRequest.Create(request, body));
        {
            // Wait for the response
            _requestAwaiter.Reset();
            await Task.Run(() => _requestAwaiter.WaitOne());
        }

        _sequence++;
        return _hwResponse;
    }

    public override uint GetPacketLength(ReadOnlySpan<byte> header)
    {
        // 28
        // 00 00 00 10
        // 00 00 00 E9
        uint databody = BinaryPrimitives.ReadUInt32BigEndian(header[5..]);
        uint headerSize = BinaryPrimitives.ReadUInt32BigEndian(header[1..]);
        return 1 + 4 + 4 + headerSize + databody + 1;
    }

    public override void OnRecvPacket(ReadOnlySpan<byte> packet)
    {
        try
        {
            // Parse the data to HwResponse
            _hwResponse = HwResponse.Parse(packet.ToArray());
            _requestAwaiter.Set();
        }
        catch
        {
            // Cleanup
            _hwResponse = null;
            _requestAwaiter.Set();
        }
    }

    public override void OnDisconnect()
    {
    }

    public override void OnSocketError(Exception e)
    {
    }
}

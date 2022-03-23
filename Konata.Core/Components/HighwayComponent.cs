using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;
using Konata.Core.Attributes;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.TcpSocket;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable MergeIntoPattern
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components;

[Component("HighwayComponent", "Konata Highway Component")]
internal class HighwayComponent : InternalComponent
{
    private const string TAG = "HighwayComponent";

    /// <summary>
    /// Upload group images
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="upload"></param>
    /// <returns></returns>
    public async Task<bool> GroupPicUp(uint selfUin,
        IEnumerable<ImageChain> upload)
    {
        // Get upload config
        var chunksize = ConfigComponent.GlobalConfig.HighwayChunkSize;
        if (chunksize is <= 1024 or > 1048576) chunksize = 8192;

        // Queue all tasks
        var tasks = new List<Task<HwResponse>>();
        foreach (var i in upload)
        {
            if (!i.PicUpInfo.UseCached)
            {
                tasks.Add(HighwayClient.Upload(
                    i.PicUpInfo.Host,
                    i.PicUpInfo.Port,
                    chunksize, selfUin,
                    i.PicUpInfo.UploadTicket,
                    i.FileData,
                    PicUp.CommandId.GroupPicDataUp
                ));
            }
        }

        LogV(TAG, "All tasks are queued, " +
                  "waiting for upload finish.");

        // Wait for tasks
        var results = await Task.WhenAll(tasks);
        return results.Count(i => i != null) == results.Length;
    }

    /// <summary>
    /// Upload private images
    /// </summary>
    /// <returns></returns>
    public async Task<bool> OffPicUp()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Multimsg upload
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="destUin"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public async Task<bool> MultiMsgUp(uint destUin, uint selfUin,
        MultiMsgChain chain)
    {
        // Queue task
        var data = new MultiMsgUpRequest(destUin,
            chain.PackedData, chain.MultiMsgUpInfo.MsgUKey);
        var task = HighwayClient.Upload(
            chain.MultiMsgUpInfo.Host,
            chain.MultiMsgUpInfo.Port,
            8192, selfUin,
            chain.MultiMsgUpInfo.UploadTicket,
            ProtoTreeRoot.Serialize(data).GetBytes(),
            PicUp.CommandId.MultiMsgDataUp
        );

        LogV(TAG, "Task queued, " +
                  "waiting for upload finish.");

        // Wait for task
        var results = await task;
        {
            var result = results.GetLeafVar("18");
            if (result != 0) return false;

            return true;
        }
    }

    /// <summary>
    /// Upload group record
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="upload"></param>
    /// <returns></returns>
    public async Task<bool> GroupPttUp(uint groupUin,
        uint selfUin, RecordChain upload)
    {
        var task = HighwayClient.Upload(
            upload.PttUpInfo.Host,
            upload.PttUpInfo.Port,
            8192, selfUin,
            upload.PttUpInfo.UploadTicket,
            upload.FileData,
            PicUp.CommandId.GroupPttDataUp,
            new GroupPttUpRequest(groupUin, selfUin, upload)
        );

        LogV(TAG, "Task queued, " +
                  "waiting for upload finish.");

        // Wait for tasks
        var results = await task;
        {
            // Get ptt id and token
            var uploadInfo = (ProtoTreeRoot) results.PathTo("3A.2A");
            {
                upload.PttUpInfo.FileKey = uploadInfo.GetLeafString("5A");
                upload.PttUpInfo.UploadId = (uint) uploadInfo.GetLeafVar("40");
            }

            return true;
        }
    }
}

internal class HighwayClient
    : AsyncClient, IClientListener
{
    private readonly uint _peer;
    private readonly byte[] _ticket;
    private int _sequence;
    private readonly Md5Cryptor _md5Cryptor;

    // We no need to use a dict to storage
    // the pending requests, cuz there's only
    // one request pending in the same time
    private HwResponse _hwResponse;
    private readonly ManualResetEvent _requestAwaiter;

    private HighwayClient(uint peer, byte[] ticket)
    {
        _peer = peer;
        _ticket = ticket;
        _sequence = new Random().Next(0x2333, 0x7090);
        _hwResponse = default;
        _requestAwaiter = new(false);
        _md5Cryptor = new();
    }

    /// <summary>
    /// Upload data
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="chunk"></param>
    /// <param name="peer"></param>
    /// <param name="ticket"></param>
    /// <param name="data"></param>
    /// <param name="cmdId"></param>
    /// <param name="extend"></param>
    /// <returns></returns>
    public static async Task<HwResponse> Upload(string host, int port, int chunk,
        uint peer, byte[] ticket, byte[] data, PicUp.CommandId cmdId, ProtoTreeRoot extend = null)
    {
        HwResponse lastResponse = null;
        var datamd5 = data.Md5();

        var client = new HighwayClient(peer, ticket);
        client.SetListener(client);
        {
            // Connect to server
            if (!await client.Connect(host, port)) return null;

            // Hello Im coming
            if (!await client.Echo()) return null;

            // Send the dataup
            var i = 0;
            while (i < data.Length)
            {
                // The remain
                if (data.Length - i < chunk)
                {
                    chunk = data.Length - i;
                }

                // DataUp
                lastResponse = await client.DataUp
                    (data, datamd5, i, chunk, cmdId, extend);
                {
                    if (lastResponse == null) return null;
                }

                i += chunk;
            }
        }

        // Disconnect after send finish
        await client.Disconnect();
        return lastResponse;
    }

    /// <summary>
    /// Send Im coming
    /// </summary>
    /// <returns></returns>
    private async Task<bool> Echo()
    {
        // Send request
        var result = await SendRequest
            (new PicUpEcho(_peer, _sequence));
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
    private async Task<HwResponse> DataUp(byte[] data, byte[] dataMd5,
        int offset, int length, PicUp.CommandId cmdId, ProtoTreeRoot extend = null)
    {
        // Calculate chunk
        var chunk = data[offset..(offset + length)];
        var chunkMD5 = _md5Cryptor.Encrypt(chunk);

        // Send request
        var result = await SendRequest(new PicUpDataUp(cmdId, _peer, _sequence,
            _ticket, data.Length, dataMd5, offset, length, chunkMD5, extend), chunk);
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

    /// <summary>
    /// Dissect a packet
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public uint OnStreamDissect(byte[] data, uint length)
    {
        // 28
        // 00 00 00 10
        // 00 00 00 E9
        if (length < 9) return 0;

        // Get the header
        var header = ByteConverter
            .BytesToUInt32(data, 1, Endian.Big);
        var databody = ByteConverter
            .BytesToUInt32(data, 1 + 4, Endian.Big);

        // Calculate the length
        // 0x28 + 8 + h + b + 0x29
        return 1 + 4 + 4 + header + databody + 1;
    }

    public void OnRecvPacket(byte[] data)
    {
        try
        {
            // Parse the data to HwResponse
            _hwResponse = HwResponse.Parse(data);
            _requestAwaiter.Set();
        }
        catch
        {
            // Cleanup
            _hwResponse = null;
            _requestAwaiter.Set();
        }
    }

    public void OnDisconnect()
    {
    }
}

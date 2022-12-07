using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.Network.TcpClient;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

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
    /// <param name="isGroup"></param>
    /// <returns></returns>
    public async Task<bool> PicDataUp(uint selfUin,
        IEnumerable<ImageChain> upload, bool isGroup)
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
                    isGroup ? PicUp.CommandId.GroupPicDataUp : PicUp.CommandId.FriendPicDataUp,
                    ConfigComponent.AppInfo
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
    /// MultiMsg upload
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
            PicUp.CommandId.MultiMsgDataUp,
            ConfigComponent.AppInfo
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

    public async Task<string> ImageOcrUp(uint selfUin, ServerInfo server, 
        byte[] ticket, byte[] image, string guid)
    {
        // Get upload config
        var chunksize = ConfigComponent.GlobalConfig.HighwayChunkSize;
        if (chunksize is <= 1024 or > 1048576) chunksize = 8192;

        // Wait for tasks
        var result = await HighwayClient.Upload(
            server.Host,
            server.Port,
            chunksize, selfUin, ticket,
            image,
            PicUp.CommandId.ImageOcrDataUp,
            ConfigComponent.AppInfo,
            new ImageOcrUpRequest(guid)
        );

        // Check result code
        var code = result.GetLeafVar("18");
        if (code != 0) return null;

        return result.GetTree("3A").GetLeafString("12");
    }

    /// <summary>
    /// Upload record
    /// </summary>
    /// <param name="destUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="upload"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    public async Task<bool> PttUp(uint destUin,
        uint selfUin, RecordChain upload, bool isGroup)
    {
        var task = HighwayClient.Upload(
            upload.PttUpInfo.Host,
            upload.PttUpInfo.Port,
            8192, selfUin,
            upload.PttUpInfo.UploadTicket,
            upload.FileData,
            isGroup ? PicUp.CommandId.GroupPttDataUp : PicUp.CommandId.FriendPttDataUp,
            ConfigComponent.AppInfo,
            isGroup
                ? new GroupPttUpRequest(ConfigComponent.AppInfo, destUin, selfUin, upload)
                : new FriendPttUpRequest(destUin, selfUin, upload)
        );

        LogV(TAG, "Task queued, " +
                  "waiting for upload finish.");

        // Wait for tasks
        var results = await task;
        {
            // Assert result is ok
            var code = results.PathTo<ProtoVarInt>("18");
            if (code != 0) return false;

            if (isGroup)
            {
                // Group ptt id and token
                var uploadInfo = results.PathTo<ProtoTreeRoot>("3A.2A");
                upload.PttUpInfo.FileKey = uploadInfo.GetLeafString("5A");
                upload.PttUpInfo.UploadId = (uint) uploadInfo.GetLeafVar("40");
            }

            else
            {
                // Friend ptt id and token
                var uploadInfo = results.PathTo<ProtoTreeRoot>("3A.3A");
                upload.PttUpInfo.FileKey = uploadInfo.GetLeafString("D205");
                upload.PttUpInfo.UploadId = uploadInfo.GetLeafBytes("A206");
            }

            return true;
        }
    }

    private class HighwayClient
        : ClientListener
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

        private HighwayClient(AppInfo appInfo, uint peer, byte[] ticket)
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
        /// Upload data
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="chunk"></param>
        /// <param name="peer"></param>
        /// <param name="ticket"></param>
        /// <param name="data"></param>
        /// <param name="cmdId"></param>
        /// <param name="appInfo"></param>
        /// <param name="extend"></param>
        /// <returns></returns>
        public static async Task<HwResponse> Upload(string host, int port, int chunk,
            uint peer, byte[] ticket, byte[] data, PicUp.CommandId cmdId, AppInfo appInfo, ProtoTreeRoot extend = null)
        {
            HwResponse lastResponse = null;
            var datamd5 = data.Md5();

            var client = new HighwayClient(appInfo, peer, ticket);
            {
                // Connect to server
                if (!await client.Connect(host, port)) return null;

                // Hello Im coming
                if (!await client.Echo()) return null;

                // Send the data up
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
            client.Disconnect();
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
        private async Task<HwResponse> DataUp(byte[] data, byte[] dataMd5,
            int offset, int length, PicUp.CommandId cmdId, ProtoTreeRoot extend = null)
        {
            // Calculate chunk
            var chunk = data[offset..(offset + length)];
            var chunkMD5 = _md5Cryptor.Encrypt(chunk);

            // Send request
            var result = await SendRequest(new PicUpDataUp(cmdId, _appInfo, _peer, _sequence,
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
}

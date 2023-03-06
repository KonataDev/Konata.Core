using System;
using System.Threading.Tasks;
using Konata.Core.Common;
using Konata.Core.Packets.Protobuf.Highway;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Network.Highway;

internal abstract class HighwayUploader
{
    public uint SelfUin { get; set; }

    public int ChunkSize { get; set; }

    public AppInfo AppInfo { get; set; }

    protected PicUp.CommandId Command { get; set; }

    /// <summary>
    /// Set upload arguments
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="appInfo"></param>
    /// <param name="chunkSize"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public HighwayUploader WithUploadArgs(uint selfUin, AppInfo appInfo, int chunkSize)
    {
        SelfUin = selfUin;
        AppInfo = appInfo;
        ChunkSize = chunkSize;



        return this;
    }

    // public abstract Task<bool> Upload();

    /// <summary>
    /// Send highway upload request
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <param name="ticket"></param>
    /// <param name="data"></param>
    /// <param name="extend"></param>
    /// <returns></returns>
    protected async Task<HwResponse> SendRequest(string host, int port,
        byte[] ticket, byte[] data, ProtoTreeRoot extend = null)
    {
        HwResponse lastResponse = null;
        var chunk = ChunkSize;
        var datamd5 = data.Md5();

        var client = new HighwayClient(AppInfo, SelfUin, ticket);
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
                    (data, datamd5, i, chunk, Command, extend);
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
}

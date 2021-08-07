using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Generic;

using Konata.Utils.IO;
using Konata.Utils.Crypto;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf.Highway;

namespace Konata.Core.Components.Model
{
    [Component("HighwayComponent", "Konata Highway Component")]
    internal class HighwayComponent : InternalComponent
    {
        private static string TAG = "HighwayComponent";

        public HighwayComponent()
        {

        }

        /// <summary>
        /// Upload group images
        /// </summary>
        /// <param name="memberUin"></param>
        /// <param name="upload"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public async Task<bool> UploadGroupImages
            (uint memberUin, ImageChain[] upload, PicUpInfo[] infos)
        {
            // Check quantities
            if (upload.Length != infos.Length)
            {
                LogV(TAG, $"Wtf? The quantity does not equal " +
                    $"upload [{upload.Length}], infos [{infos.Length}]");
                return false;
            }

            // Get upload config
            var chunksize = ConfigComponent.GlobalConfig.ImageChunkSize;
            {
                // Length limit
                if (chunksize <= 1024 || chunksize > 1048576)
                {
                    chunksize = 8192;
                }
            }

            // Queue all tasks
            var tasks = new List<Task<bool>>();
            for (int i = 0; i < upload.Length; ++i)
            {
                if (!infos[i].UseCached)
                {
                    tasks.Add(TaskUploadGroup(memberUin, upload[i], infos[i], chunksize));
                }
            }

            // Wait for tasks
            LogV(TAG, "All tasks are queued, waiting for upload finished.");
            Task.WaitAll(tasks.ToArray());

            return true;
        }

        /// <summary>
        /// Upload private images
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadPrivateImages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Image upload task
        /// </summary>
        /// <param name="upload"></param>
        /// <param name="info"></param>
        private async Task<bool> TaskUploadGroup(uint memberUin,
            ImageChain upload, PicUpInfo info, int chunksize)
        {
            // Okay lets do upload
            var client = new PicUpClient(memberUin, info.ServiceTicket);
            {
                // Connect to server
                if (!client.Connect(info.Host, info.Port))
                {
                    return false;
                }

                // Heartbreak
                if (!client.Echo())
                {
                    return false;
                }

                // Send the dataup
                var i = 0;
                while (i < upload.FileLength)
                {
                    // The remain
                    if (upload.FileLength - i < chunksize)
                    {
                        chunksize = (int)upload.FileLength - i;
                    }

                    // DataUp
                    client.DataUp(upload.FileData,
                        upload.HashData, i, chunksize);

                    i += chunksize;
                }

            }
            client.Close();

            return true;
        }
    }

    [Obsolete]
    internal class PicUpClient
    {
        private uint _peer;
        private byte[] _ticket;
        private int _sequence;
        private Socket _socket;
        private byte[] _recvBuffer;

        public PicUpClient(uint peer, byte[] ticket)
        {
            _peer = peer;
            _ticket = ticket;

            _sequence = new Random().Next(0x2333, 0x7090);
            _recvBuffer = new byte[1048576];
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.ReceiveBufferSize = 1048576;
        }

        public bool Connect(string host, int port)
        {
            _socket.ConnectAsync(host, port).Wait();
            return _socket.Connected;
        }

        public void Close()
        {
            _socket.Close();
            _socket.Dispose();
        }

        /// <summary>
        /// Send echo
        /// </summary>
        /// <returns></returns>
        public bool Echo()
        {
            var echo = new PicUpEcho(_peer, ++_sequence);

            // Send and recv
            // TODO: fix receive
            _socket.Send(HwRequest.Create(echo));
            Thread.Sleep(2000);

            var len = _socket.Receive(_recvBuffer);
            Console.WriteLine($"DataUp ret => {ByteConverter.Hex(_recvBuffer.Take(len).ToArray())}");

            return true;
        }

        /// <summary>
        /// Send DataUp
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataMD5"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool DataUp(byte[] data, byte[] dataMD5,
            int offset, int length)
        {
            // Calculate chunk
            var chunk = data[offset..(offset + length)];
            var chunkMD5 = new Md5Cryptor().Encrypt(chunk);

            // Build PicUp
            var dataup = new PicUpDataUp(_peer,
                ++_sequence, _ticket,
                data.Length, dataMD5,
                offset, length, chunkMD5);

            // Send and recv
            // TODO: fix receive
            _socket.Send(HwRequest.Create(dataup, chunk));
            Thread.Sleep(2000);

            var len = _socket.Receive(_recvBuffer);
            Console.WriteLine($"DataUp ret => {ByteConverter.Hex(_recvBuffer.Take(len).ToArray())}");

            return true;
        }
    }
}

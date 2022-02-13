using Konata.Core.Attributes;
using Konata.Core.Entity;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.TcpSocket;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InvertIf
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace Konata.Core.Components.Model
{
    [Component("SocketComponent", "Konata Socket Client Component")]
    internal class SocketComponent
        : InternalComponent, IClientListener
    {
        private static (string Host, int Port)[] DefaultServers { get; } =
        {
            new("msfwifi.3g.qq.com", 8080),
            new("14.215.138.110", 8080),
            new("113.96.12.224", 8080),
            new("157.255.13.77", 14000),
            new("120.232.18.27", 443),
            new("183.3.235.162", 14000),
            new("163.177.89.195", 443),
            new("183.232.94.44", 80),
            new("203.205.255.224", 8080),
            new("203.205.255.221", 8080),
        };

        public bool Connected
            => _tcpClient.Connected;

        private const string TAG = "SocketComponent";
        private readonly AsyncClient _tcpClient;

        public SocketComponent()
        {
            _tcpClient = new(this);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="useLowLatency"><b>[Opt] </b>Auto select the fastest server to connect.</param>
        /// <returns></returns>
        public async Task<bool> Connect(bool useLowLatency = false)
        {
            // Check socket
            if (_tcpClient.Connected)
            {
                LogW(TAG, "Calling Connect method after socket connected.");
                return true;
            }

            var lowestTime = long.MaxValue;
            var serverList = new ((string, int), long)[] { }.ToList();

            // Using user config
            if (ConfigComponent.GlobalConfig!.CustomHost != null)
            {
                // Parse the config
                var customHost = ConfigComponent
                    .GlobalConfig.CustomHost.Split(':');

                // Connect to server with
                // custom server address
                if (customHost.Length >= 1)
                {
                    return await _tcpClient.Connect(customHost[0],
                        customHost.Length == 2 ? ushort.Parse(customHost[1]) : 8080);
                }

                // Failed to parse the config
                LogE(TAG, "Invalid custom host config passed in.");
                return false;
            }

            // Using fastest server
            else if (useLowLatency)
            {
                // Ping the server
                foreach (var server in DefaultServers)
                {
                    var time = Icmp.Ping(server.Host, 2000);
                    {
                        if (time < lowestTime) lowestTime = time;
                        serverList.Add(((server.Host, server.Port), time));
                    }

                    LogI(TAG, "Probing latency " +
                              $"{server.Host}:{server.Port} " +
                              $"=> {time}ms.");
                }

                // Sort the list by lantency
                serverList.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                var tryQueue = new Queue(serverList);

                // Try connect to each server
                while (tryQueue.Count > 0)
                {
                    var pop = (((string, int ) Host, long Latency)) tryQueue.Dequeue();
                    var (Addr, Port) = pop.Host;

                    // Connect
                    LogI(TAG, $"Try Connecting {Addr}:{Port}.");
                    var result = await _tcpClient.Connect(Addr, Port);

                    // Try next server
                    if (result) return true;
                    {
                        LogI(TAG, $"Failed to connecting to {Addr}:{Port}.");
                        await _tcpClient.Disconnect();
                    }
                }
            }

            throw new Exception("All servers are unavailable.");
        }

        /// <summary>
        /// Reconnect
        /// </summary>
        /// <returns></returns>
        public Task<bool> Reconnect()
            => _tcpClient.Reconnect();

        /// <summary>
        /// Disconnect from server
        /// </summary>
        /// <param name="reason"></param>
        public async Task<bool> Disconnect(string reason)
        {
            // Not connected
            if (_tcpClient is not {Connected: true})
            {
                LogW(TAG, "Calling Disconnect " +
                          "method after socket disconnected.");
                return true;
            }

            // Disconnect
            LogI(TAG, $"Disconnect, Reason => {reason}");
            return await _tcpClient.Disconnect();
        }

        /// <summary>
        /// On dissecting a packet
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public uint OnStreamDissect(byte[] data, uint length)
        {
            // Lack more data
            if (length < 4) return 0;

            // Teardown the packet length
            return BitConverter.ToUInt32
                (data.Take(4).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// On Received a packet 
        /// </summary>
        /// <param name="data"></param>
        public async void OnRecvPacket(byte[] data)
        {
            var packet = new byte[data.Length];
            Array.Copy(data, 0, packet, 0, data.Length);
            {
                LogV(TAG, $"Recv data => \n  {ByteConverter.Hex(packet, true)}");

                try
                {
                    await PushNewPacket(this, packet);
                }
                catch (Exception e)
                {
                    LogE(TAG, e);
                }
            }
        }

        /// <summary>
        /// On disconnect
        /// </summary>
        public void OnDisconnect()
        {
            // Push offline
            PushOffline(this, "Client disconnected.");
        }

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="task"></param>
        internal override async Task<bool> OnHandleEvent(KonataTask task)
        {
            if (task.EventPayload is PacketEvent packetEvent)
            {
                // Not connected
                if (_tcpClient is not {Connected: true})
                {
                    LogW(TAG, "Calling SendData method after socket disconnected.");
                }

                // Send data
                await _tcpClient.Send(packetEvent.Buffer);
                LogV(TAG, $"Send data => \n  {ByteConverter.Hex(packetEvent.Buffer, true)}");
            }
            else LogW(TAG, "Unsupported event received.");

            return false;
        }

        #region Stub methods

        private static Task PushOffline(SocketComponent context, string reason)
            => context.PostEvent<BusinessComponent>(OnlineStatusEvent.Push(OnlineStatusEvent.Type.Offline, reason));

        private static Task PushNewPacket(SocketComponent context, byte[] data)
            => context.PostEvent<PacketComponent>(PacketEvent.Push(data));

        #endregion
    }
}

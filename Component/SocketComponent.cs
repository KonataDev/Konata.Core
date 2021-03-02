using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using Konata.Utils;
using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Entity;
using System.Threading;
using Konata.Utils.IO;

namespace Konata.Core.Component
{
    [Component("SocketComponent", "Konata Socket Client Component")]
    public class SocketComponent : BaseComponent
    {
        private enum ReceiveStatus
        {
            Idle,
            RecvBody,
            Stop
        }

        private struct ServerInfo
        {
            public string Host;
            public int Port;
        }

        private static ServerInfo[] DefaultServers { get; } =
        {
            new ServerInfo { Host = "msfwifi.3g.qq.com", Port = 8080 },
            new ServerInfo { Host = "14.215.138.110", Port = 8080 },
            new ServerInfo { Host = "113.96.12.224", Port = 8080 },
            new ServerInfo { Host = "157.255.13.77", Port = 14000 },
            new ServerInfo { Host = "120.232.18.27", Port = 443 },
            new ServerInfo { Host = "183.3.235.162", Port = 14000 },
            new ServerInfo { Host = "163.177.89.195", Port = 443 },
            new ServerInfo { Host = "183.232.94.44", Port = 80 },
            new ServerInfo { Host = "203.205.255.224", Port = 8080 },
            new ServerInfo { Host = "203.205.255.221", Port = 8080 },
        };

        public string TAG = "SocketComponent";

        private Socket _socket;

        private int _packetLength;

        private int _recvLength;
        private byte[] _recvBuffer;
        private ReceiveStatus _recvStatus;

        ~SocketComponent()
            => _socket.Dispose();

        public SocketComponent()
        {
            _recvBuffer = new byte[2048];
            _recvStatus = ReceiveStatus.Idle;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="useLowLentency"><b>[Opt] </b>Auto select low letency server to connect.</param>
        /// <returns></returns>
        public Task<bool> Connect(bool useLowLentency = false)
        {
            var lowestTime = long.MaxValue;
            var selectHost = DefaultServers[0];

            if (useLowLentency)
            {
                foreach (var item in DefaultServers)
                {
                    var time = Network.PingTest(item.Host, 2000);
                    {
                        if (time < lowestTime)
                        {
                            lowestTime = time;
                            selectHost = item;
                        }
                    }
                }
            }

            return Connect(selectHost.Host, selectHost.Port);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="useLowLentency"><b>[Opt] </b>Auto select low letency server to connect.</param>
        /// <returns></returns>
        public Task<bool> Connect(string hostIp, int port)
        {
            try
            {
                _socket.BeginConnect(hostIp, port, BeginConnect, null)
                    .AsyncWaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                LogE(TAG, "Connect failed.");
                LogE(TAG, e);
            }

            return Task.FromResult(_socket.Connected);
        }

        /// <summary>
        /// Begin connect
        /// </summary>
        /// <param name="result"></param>
        private void BeginConnect(IAsyncResult result)
        {
            try
            {
                _socket.EndConnect(result);
                _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, BeginReceive, null);
            }
            catch (Exception e)
            {
                LogE(TAG, "BeginConnect failed.");
                LogE(TAG, e);
            }
        }

        /// <summary>
        /// Begin receive data
        /// </summary>
        /// <param name="result"></param>
        private void BeginReceive(IAsyncResult result)
        {
            try
            {
                _recvLength += _socket.EndReceive(result);
            // Console.WriteLine(ByteConverter.Hex(_recvBuffer));

            ReceiveNext:

                // If receive status is Idle
                if (_recvStatus == ReceiveStatus.Idle)
                {
                    // If receive length too short
                    if (_recvLength < 4)
                    {
                        Thread.Sleep(10);
                        return;
                    }

                    // Take out first 4 bytes as this packet length
                    _packetLength = BitConverter.ToInt32(_recvBuffer.Take(4).Reverse().ToArray(), 0);
                    if (_packetLength > _recvBuffer.Length)
                        Array.Resize(ref _recvBuffer, _packetLength);

                    // Prepare for receive packet body
                    _recvStatus = ReceiveStatus.RecvBody;
                }

                // If ready for receive packet body
                if (_recvStatus == ReceiveStatus.RecvBody)
                {
                    // If receive length bigger than packet langth
                    // Means we have received at least 1 packet
                    if (_recvLength >= _packetLength)
                    {
                        // Process this packet
                        OnReceivePacket(_recvBuffer, _packetLength);

                        // Calculate remain length
                        _recvStatus = ReceiveStatus.Idle;
                        _recvLength = _recvLength - _packetLength;

                        // If next packet existed
                        if (_recvLength > 0)
                        {
                            // Move remain data to ahead
                            Array.Copy(_recvBuffer, _packetLength, _recvBuffer, 0, _recvLength);

                            // Process it again!
                            goto ReceiveNext;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogE(TAG, e);
                DisConnect($"Socket error while receiving data. ");
            }
            finally
            {
                if (_recvStatus != ReceiveStatus.Stop)
                {
                    _socket.BeginReceive(_recvBuffer, _recvLength, _recvBuffer.Length - _recvLength,
                        SocketFlags.None, BeginReceive, null);
                }
            }
        }

        private void BeginSendData(IAsyncResult result)
        {
            try
            {
                _socket.EndSend(result);
            }
            catch (Exception e)
            {
                LogE(TAG, e);
                DisConnect($"Socket error while sending data.");
            }
        }

        /// <summary>
        /// On Received a packet 
        /// </summary>
        /// <param name="buffer"></param>
        private void OnReceivePacket(byte[] buffer, int length)
        {
            var packet = new byte[length];
            {
                Array.Copy(buffer, 0, packet, 0, length);
                PostEvent<PacketComponent>(new PacketEvent
                {
                    Buffer = packet,
                    EventType = PacketEvent.Type.Receive
                });

                LogV(TAG, $"Recv data => \n  { ByteConverter.Hex(packet, true) }");
            }
        }

        /// <summary>
        /// Send packet to server
        /// </summary>
        /// <param name="buffer"><b>[In] </b>Data buffer to send</param>
        /// <returns></returns>
        public Task<bool> SendData(byte[] buffer)
        {
            if (!_socket.Connected)
            {
                LogW(TAG, "Calling SendData method after socket disconnected.");
                return Task.FromResult(false);
            }

            try
            {
                LogV(TAG, $"Send data => \n  { ByteConverter.Hex(buffer, true) }");
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, BeginSendData, null);
            }
            catch (Exception e)
            {
                LogE(TAG, e);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        /// <param name="reason"></param>
        public Task<bool> DisConnect(string reason)
        {
            if (!_socket.Connected)
            {
                LogW(TAG, "Calling DisConnect method after socket disconnected.");
                return Task.FromResult(false);
            }

            _socket.Close();
            _recvStatus = ReceiveStatus.Stop;

            PostEvent<BusinessComponent>(new OnlineStatusEvent
            {
                EventType = OnlineStatusEvent.Type.Offline,
                EventMessage = reason
            });

            return Task.FromResult(true);
        }

        internal override void EventHandler(KonataTask task)
        {
            if (task.EventPayload is PacketEvent packetEvent)
            {
                SendData(packetEvent.Buffer);
            }
            else
            {
                LogW(TAG, "Unsupported event received.");
            }
        }
    }
}

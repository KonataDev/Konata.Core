using System;
using System.Linq;
using System.Net.Sockets;
using Konata.Library.IO;
using Konata.Msf.Packets;
using Konata.Utils;

namespace Konata.Msf.Network
{
    internal class PacketMan
    {
        private enum ReceiveStatus
        {
            Idle,
            RecvBody,
            Stop
        }

        private struct MsfServer
        {
            public string url;
            public ushort port;
        }

        private static MsfServer[] _msfServers =
        {
            new MsfServer { url = "127.0.0.1", port = 8080 },
            new MsfServer { url = "msfwifi.3g.qq.com", port = 8080 },
            new MsfServer { url = "14.215.138.110", port = 8080 },
            new MsfServer { url = "113.96.12.224", port = 8080 },
            new MsfServer { url = "157.255.13.77", port = 14000 },
            new MsfServer { url = "120.232.18.27", port = 443 },
            new MsfServer { url = "183.3.235.162", port = 14000 },
            new MsfServer { url = "163.177.89.195", port = 443 },
            new MsfServer { url = "183.232.94.44", port = 80 },
            new MsfServer { url = "203.205.255.224", port = 8080 },
            new MsfServer { url = "203.205.255.221", port = 8080 },
        };

        private SsoMan ssoMan;
        private Socket socket;

        private int packetLength;

        private int recvLength;
        private byte[] recvBuffer;
        private ReceiveStatus recvStatus;

        public PacketMan(SsoMan ssoMan)
        {
            this.ssoMan = ssoMan;
        }

        /// <summary>
        /// 打開網路插座
        /// </summary>
        /// <returns></returns>
        public bool OpenSocket()
        {
            recvBuffer = new byte[2048];
            recvStatus = ReceiveStatus.Idle;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_msfServers[0].url, _msfServers[0].port);
            socket.BeginReceive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None, OnReceive, null);

            return true;
        }

        /// <summary>
        /// 關閉網路插座
        /// </summary>
        /// <returns></returns>
        public bool CloseSocket()
        {
            if (recvStatus == ReceiveStatus.Stop)
            {
                return false;
            }

            socket.Close();
            recvStatus = ReceiveStatus.Stop;
            return true;
        }

        public bool Emit(ServiceMessage message)
        {
            var packet = message.BuildToService();
            var packetLen = packet.Length;

            var sendBuffer = new ByteBuffer();
            {
                sendBuffer.PutUintBE(packetLen);
                sendBuffer.PutByteBuffer(packet);
            }
            return OnSend(sendBuffer.GetBytes());
        }

        private bool OnSend(byte[] data)
        {
            if (recvStatus == ReceiveStatus.Stop)
                return false;

            Console.WriteLine($"Send =>\n{Hex.Bytes2HexStr(data)}\n");
            return socket.Send(data) != 0;
        }

        private void OnPacket(byte[] data)
        {
            var packet = new ByteBuffer(data);
            {
                packet.PeekUintBE(out var length);
                {
                    if (length != packet.Length)
                        throw new Exception("Invalid packet received.");
                    packet.TakeUintBE(out length);
                }

                var fromService = new ServiceMessage(packet.TakeAllBytes(out var _));
                Console.WriteLine($"Recv =>\n{Hex.Bytes2HexStr(data)}");
                Console.WriteLine($"  [FromService] len => {data.Length}");
                Console.WriteLine($"  [FromService] pktType => {fromService.GetPacketType()}");
                Console.WriteLine($"  [FromService] pktFlag => {fromService.GetPacketFlag()}");
                Console.WriteLine($"  [FromService] uin => {fromService.GetUin()}");

                ssoMan.OnFromServiceMessage(fromService);
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            try
            {
                recvLength += socket.EndReceive(result);

                if (recvStatus == ReceiveStatus.Idle)
                {
                    if (recvLength < 4) return;
                    packetLength = BitConverter.ToInt32(recvBuffer.Take(4).Reverse().ToArray(), 0);
                    Array.Resize(ref recvBuffer, packetLength);
                    recvStatus = ReceiveStatus.RecvBody;
                }

                if (recvStatus == ReceiveStatus.RecvBody)
                {
                    if (recvLength == packetLength)
                    {
                        recvLength = 0;
                        recvStatus = ReceiveStatus.Idle;

                        OnPacket(recvBuffer);
                    }
                }
            }
            catch
            {
                CloseSocket();
            }
            finally
            {
                if (recvStatus != ReceiveStatus.Stop)
                {
                    socket.BeginReceive(recvBuffer, recvLength, recvBuffer.Length - recvLength,
                        SocketFlags.None, OnReceive, null);
                }
            }
        }
    }
}

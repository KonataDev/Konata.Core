using System;
using System.Linq;
using System.Net.Sockets;
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
            // new MsfServer { url = "127.0.0.1", port = 8080 },
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

        private SsoMan _ssoMan;
        private Socket _socket;

        private int _packetLength;

        private int _recvLength;
        private byte[] _recvBuffer;
        private ReceiveStatus _recvStatus;

        public PacketMan(SsoMan ssoMan)
        {
            _ssoMan = ssoMan;
        }

        /// <summary>
        /// 打開網路插座
        /// </summary>
        /// <returns></returns>
        public bool OpenSocket()
        {
            _recvBuffer = new byte[2048];
            _recvStatus = ReceiveStatus.Idle;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_msfServers[0].url, _msfServers[0].port);
            _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, OnReceive, null);

            return true;
        }

        /// <summary>
        /// 關閉網路插座
        /// </summary>
        /// <returns></returns>
        public bool CloseSocket()
        {
            if (_recvStatus == ReceiveStatus.Stop)
            {
                return false;
            }

            _socket.Close();
            _recvStatus = ReceiveStatus.Stop;
            return true;
        }

        public void Emit(ToServiceMessage message)
        {
            OnSend(message.GetBytes());
        }

        private void OnSend(byte[] data)
        {
            if (_recvStatus == ReceiveStatus.Stop)
            {
                return;
            }

            _socket.Send(data);
            Console.WriteLine($"Send =>\n{Hex.Bytes2HexStr(data)}\n");
        }

        private void OnReceive(IAsyncResult result)
        {
            try
            {
                _recvLength += _socket.EndReceive(result);

                if (_recvStatus == ReceiveStatus.Idle)
                {
                    if (_recvLength < 4) return;
                    _packetLength = BitConverter.ToInt32(_recvBuffer.Take(4).Reverse().ToArray(), 0);
                    Array.Resize(ref _recvBuffer, _packetLength);
                    _recvStatus = ReceiveStatus.RecvBody;
                }

                if (_recvStatus == ReceiveStatus.RecvBody)
                {
                    if (_recvLength == _packetLength)
                    {
                        _recvLength = 0;
                        _recvStatus = ReceiveStatus.Idle;

                        OnPacket(_recvBuffer);
                    }
                }
            }
            catch
            {
                CloseSocket();
            }
            finally
            {
                if (_recvStatus != ReceiveStatus.Stop)
                {
                    _socket.BeginReceive(_recvBuffer, _recvLength, _recvBuffer.Length - _recvLength,
                        SocketFlags.None, OnReceive, null);
                }
            }
        }

        private void OnPacket(byte[] data)
        {
            var serviceMessage = new FromServiceMessage(data);

            Console.WriteLine($"Recv =>\n{Hex.Bytes2HexStr(data)}");
            Console.WriteLine($"  [ToService] len => {serviceMessage._length}");
            Console.WriteLine($"  [ToService] packetType => {serviceMessage._packetType}");
            Console.WriteLine($"  [ToService] encryptType => {serviceMessage._encryptType}");
            Console.WriteLine($"  [ToService] uinString => {serviceMessage._uinString}");

            _ssoMan.OnFromServiceMessage(serviceMessage);
        }
    }
}

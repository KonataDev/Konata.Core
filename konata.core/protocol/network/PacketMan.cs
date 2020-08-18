using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Konata.Network.Msf;
using Konata.Protocol.Packet;
using Konata.Utils;

namespace Konata.Network
{
    public class PacketMan
    {
        public delegate void PacketListener(PacketBase packet);

        private enum ReceiveStatus
        {
            Idle,
            RecvBody,
            Stop
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

        private Queue<PacketBase> _packets;
        private PacketListener _listener;
        private Socket _socket;
        // private Thread _thread;

        private int _packetLength;

        private int _recvLength;
        private byte[] _recvBuffer;
        private ReceiveStatus _recvStatus;

        public PacketMan(PacketListener listener)
        {
            _listener = listener;
        }

        public void Init()
        {
            _recvBuffer = new byte[2048];

            _recvStatus = ReceiveStatus.Idle;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_msfServers[0].url, _msfServers[0].port);
            _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, OnReceive, null);

            //_thread = new Thread(PacketManThread);
            //_thread.Start();

        }

        public void ShutDown()
        {
            _socket.Close();
            _recvStatus = ReceiveStatus.Stop;
            // _thread.Join();
        }

        public void Emit(PacketBase packet)
        {
            // _packets.Enqueue(packet);
            _socket.Send(packet.GetBytes());

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

                        Console.WriteLine(Hex.Bytes2HexStr(_recvBuffer));
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                _socket.BeginReceive(_recvBuffer, _recvLength, _recvBuffer.Length - _recvLength, SocketFlags.None, OnReceive, null);
            }

        }
    }
}

using System;
using System.Text;
using System.Collections.Concurrent;

using Konata.Runtime.Base;
using Konata.Runtime.Builder;
using Konata.Runtime.Network;
using Konata.Runtime.Extensions;
using System.Collections.Generic;

namespace Konata.Core.Service
{
    [Service("Socket管理服务", "Socket统一管理服务")]
    public class SocketService : ILoad, IDisposable
    {
        private ConcurrentDictionary<Entity, ISocket> _socketList = null;

        public void Load()
        {
            if (_socketList == null)
            {
                _socketList = new ConcurrentDictionary<Entity, ISocket>();
            }
        }

        private int SocketLenCalcer(List<byte> bytes)
        {
            if (bytes.Count < 3)
            {
                return -1;
            }
            byte[] lenBytes = bytes.GetRange(0, 3).ToArray();

            return BitConverter.ToInt16(lenBytes, 0);
        }

        public ISocket CreateNewSocketInstance(Entity entity, SocketConfig config)
        {
            if (_socketList.TryGetValue(entity, out var socket))
            {
                return socket;
            }

            PacketService service = ServiceManager.Instance.GetService<PacketService>();

            socket = new SocketBuilder()
                .SocketConfig(conf =>
                {
                    conf = config;
                })
                .SetServerCloseWatcher(() =>
                {
                    //Console.WriteLine("socket已关闭");
                })
                .SetRecvLenCalcer((bufferlist) =>
                {
                    return SocketLenCalcer(bufferlist);
                })
                .SetServerDataReceiver((bytes) =>
                {
                    service.SendSocketData(new SocketPackage { Data = bytes, Owner = entity });
                })
                .Build();

            _socketList.TryAdd(entity, socket);
            return socket;
        }

        public void SendData(Entity entity, byte[] data)
        {
            if (_socketList.TryGetValue(entity, out ISocket socket)
                && socket.Connected)
            {
                socket.Send(data);
            }
        }

        public bool RegisterNewSocket(Entity entity, ISocket socket)
        {
            return _socketList.TryAdd(entity, socket);
        }

        public bool UnRegisterSocket(Entity entity)
        {
            return _socketList.TryRemove(entity, out var _);
        }

        public void UnloadSocketInstance(Entity entity)
        {
            if (_socketList.TryGetValue(entity, out var socket))
            {
                if (socket.Connected)
                {
                    socket.DisConnect();
                }
            }
        }

        public void Dispose()
        {
            if (_socketList != null)
            {
                foreach (var a in _socketList.Values)
                {
                    a.DisConnect();
                }
                _socketList.Clear();
                _socketList = null;
            }
        }
    }
}

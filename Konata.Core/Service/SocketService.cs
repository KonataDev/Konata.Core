using System;
using System.Text;
using System.Collections.Generic;

using Konata.Runtime.Base;
using Konata.Runtime.Builder;
using Konata.Runtime.Network;
using Konata.Runtime.Extensions;

namespace Konata.Core.Service
{
    [Service("Socket管理服务", "Socket统一管理服务")]
    public class SocketService : ILoad, IDisposable
    {
        private Dictionary<Entity, ISocket> _socketList = null;

        public void Load()
        {
            if (_socketList == null)
            {
                _socketList = new Dictionary<Entity, ISocket>();
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

            _socketList.Add(entity, socket);
            return socket;
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

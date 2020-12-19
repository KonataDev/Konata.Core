using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Konata.Core.Manager;
using Konata.Runtime.Base;
using Konata.Runtime.Builder;
using Konata.Runtime.Network;
using Konata.Runtime.Extensions;

namespace Konata.Core.Service
{
    [Service("Socket管理服务", "Socket统一管理服务")]
    public class SocketService : ILoad, IDisposable
    {
        private ReaderWriterLockSlim _locker;
        private Dictionary<Entity, SocketComponent> _socketList = null;

        public void Load()
        {
            if (_socketList == null)
            {
                _socketList = new Dictionary<Entity, SocketComponent>();
                _locker = new ReaderWriterLockSlim();
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

        /// <summary>
        /// 创建新的Socket实例
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public SocketComponent CreateNewSocketInstance(Entity entity, SocketConfig config)
        {
            _locker.EnterUpgradeableReadLock();
            try
            {
                if (_socketList.TryGetValue(entity, out var com))
                {
                    return com;
                }

                PacketService service = ServiceManager.Instance.GetService<PacketService>();

                _locker.EnterWriteLock();
                try
                {
                    ISocket socket = new SocketBuilder()
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
                    SocketComponent scom= ComponentFactory.Create<SocketComponent>();
                    scom.Socket = socket;
                    _socketList.Add(entity, scom);
                    return scom;
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 卸载目标实体的Socket实例
        /// </summary>
        /// <param name="entity"></param>
        public void UnloadSocketInstance(Entity entity)
        {
            _locker.EnterWriteLock();
            try
            {
                if (_socketList.TryGetValue(entity, out var com))
                {
                    if (com.Socket != null && com.Socket.Connected)
                    {
                        com.Socket.DisConnect();
                    }
                    _socketList.Remove(entity);
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        /// <summary>
        /// 向指定实体socket发送socket消息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="data"></param>
        public void SendSocketPackage(Entity entity,byte[] data)
        {
            _locker.EnterReadLock();
            try
            {
                if(_socketList.TryGetValue(entity,out var com)&&com.Socket!=null&&com.Socket.Connected)
                {
                    com.Socket.Send(data);
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _locker.EnterWriteLock();
            try
            {
                if (_socketList != null)
                {
                    foreach (var a in _socketList.Values)
                    {
                        if (a.Socket != null && a.Socket.Connected)
                        {
                            a.Socket.DisConnect();
                        }
                        a.Socket = null;
                        a.Parent.RemoveComponent<SocketComponent>();
                    }
                    _socketList.Clear();
                    _socketList = null;
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }

        }
    }
}

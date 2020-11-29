using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using Konata.Core.Packet;
using Konata.Core.EventArgs;
using Konata.Runtime.Base;
using Konata.Runtime.Base.Event;
using Konata.Runtime.Network;

namespace Konata.Core
{
    [Service("消息解析服务", "全局消息解析服务，用于消息序列化/反序列化")]
    class PacketService : ILoad, IDisposable
    {
        private List<SSOServiceAttribute> _ssoServiceInfo = null;
        private Dictionary<string, ISSOService> _ssoServiceList = null;

        private TransformBlock<ServiceMessage, SSOMessage> _serviceMsgTransformBlock;
        private TransformBlock<SocketPackage, ServiceMessage> _socketMsgTransformBlock;

        private ActionBlock<SSOMessage> _ssoMsgActionBlock;
        private ActionBlock<KonataEventArgs> _eventActionBlock;

        private ConcurrentDictionary<long, ISocket> _entitySocketList = null;
        private ConcurrentDictionary<long, ActionBlock<KonataEventArgs>> _entityEventActionBlock;

        /// <summary>
        /// Service Onload
        /// </summary>
        public void Load()
        {
            if (_ssoServiceList == null)
            {
                _ssoServiceInfo = new List<SSOServiceAttribute>();
                _ssoServiceList = new Dictionary<string, ISSOService>();
                _entitySocketList = new ConcurrentDictionary<long, ISocket>();
                _entityEventActionBlock = new ConcurrentDictionary<long, ActionBlock<KonataEventArgs>>();

                // Load all of the workers with specific attribute
                foreach (Type type in typeof(PacketService).Assembly.GetTypes())
                {
                    var attribute = (SSOServiceAttribute)type.GetCustomAttributes(typeof(SSOServiceAttribute)).FirstOrDefault();
                    if (attribute != null && typeof(ISSOService).IsAssignableFrom(type))
                    {
                        var service = (ISSOService)Activator.CreateInstance(type);
                        _ssoServiceList.Add(attribute.ServiceName, service);
                        _ssoServiceInfo.Add(attribute);
                    }
                }

                // [Incoming] Working pipeline
                //   Socket -> ServiceMessage
                _socketMsgTransformBlock = new TransformBlock<SocketPackage, ServiceMessage>
                    (socketData => ServiceMessage.Parse(socketData, out var serviceMsg) ? serviceMsg : null);

                // [Incoming] Working pipeline
                //   ServiceMessage -> SSOMessage
                _serviceMsgTransformBlock = new TransformBlock<ServiceMessage, SSOMessage>
                    (serviceMsg => SSOMessage.Parse(serviceMsg, out var ssoMessage) ? ssoMessage : null);

                // [Incoming] Action pipeline
                //   SSOMessage -> SSO Service
                _ssoMsgActionBlock = new ActionBlock<SSOMessage>
                (ssoMessage =>
                {
                    // Get packet worker by sso command
                    if (_ssoServiceList.TryGetValue(ssoMessage.Command, out ISSOService service))
                    {
                        // DeSerialize the packet
                        if (service.DeSerialize(ssoMessage, out var arg))
                        {
                            // Post data to target service entity
                            if (_entityEventActionBlock.TryGetValue(ssoMessage.Receiver.Id, out var action))
                            {
                                action.SendAsync(arg);
                            }
                        }
                    }
                });

                // [Incoming] Connect the working pipelines up
                //   ServiceMessage -> SSOMessage -> Service Entity
                _socketMsgTransformBlock.LinkTo(_serviceMsgTransformBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoMessage => ssoMessage != null);

                _serviceMsgTransformBlock.LinkTo(_ssoMsgActionBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoMessage => ssoMessage != null);

                // [OutGoing] Action pipeline
                //   SSO Service -> Socket
                _eventActionBlock = new ActionBlock<KonataEventArgs>
                (eventMessage =>
                {
                    // Get service by sso command
                    if (_ssoServiceList.TryGetValue(eventMessage.EventName, out ISSOService worker))
                    {
                        // Serialize the packet
                        if (worker.Serialize(eventMessage, out var data))
                        {
                            if (data != null
                                && _entitySocketList.TryGetValue(eventMessage.Receiver.Id, out ISocket socket)
                                && socket.Connected)
                            {
                                socket.Send(data);
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 发送socket消息包
        /// </summary>
        /// <param name="package"></param>
        /// <param name="timeoutMs"></param>
        public async void SendSocketData(SocketPackage package, int timeoutMs = 0)
        {
            if (_socketMsgTransformBlock != null && package != null)
            {
                if (timeoutMs > 0)
                {
                    var source = new CancellationTokenSource(timeoutMs);
                    await _socketMsgTransformBlock.SendAsync(package, source.Token);
                }
                else
                {
                    await _socketMsgTransformBlock.SendAsync(package);
                }
            }
        }

        /// <summary>
        /// 将事件消息发送到
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="timeoutMs"></param>
        public async void SendDataToServer(KonataEventArgs eventArgs, int timeoutMs = 0)
        {
            if (_socketMsgTransformBlock != null && eventArgs != null)
            {
                if (timeoutMs > 0)
                {
                    var source = new CancellationTokenSource(timeoutMs);
                    await _eventActionBlock.SendAsync(eventArgs, source.Token);
                }
                else
                {
                    await _eventActionBlock.SendAsync(eventArgs);
                }
            }
        }

        /// <summary>
        /// 注册新的实体接收管道
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public bool RegisterNewReceiver(Entity entity, ActionBlock<KonataEventArgs> pipe)
        {
            return _entityEventActionBlock.TryAdd(entity.Id, pipe);
        }

        /// <summary>
        /// 移除指定实体接收管道
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UnRegisterReceiver(Entity entity)
        {
            return _entityEventActionBlock.TryRemove(entity.Id, out var _);
        }

        public bool RegisterNewSocket(Entity entity, ISocket socket)
        {
            return _entitySocketList.TryAdd(entity.Id, socket);
        }

        public bool UnRegisterSocket(Entity entity)
        {
            return _entitySocketList.TryRemove(entity.Id, out var _);
        }

        public void Dispose()
        {
            //socket-servicemsg-ssomsg linked with PropagateCompletion
            //Source Link Complete signal will send to it target
            _socketMsgTransformBlock.Complete();
            _ssoServiceList.Clear();
            _ssoServiceList = null;

            foreach (var data in _entityEventActionBlock.Values)
            {
                data.Complete();
            }
            _entityEventActionBlock.Clear();
            _entityEventActionBlock = null;
        }
    }
}

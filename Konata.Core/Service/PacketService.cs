using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Runtime.Base;
using Konata.Runtime.Base.Event;
using Konata.Runtime.Network;

namespace Konata.Core.Service
{
    [Service("消息解析服务", "全局消息解析服务，用于消息序列化/反序列化")]
    class PacketService : ILoad, IDisposable
    {
        private List<SSOServiceAttribute> _ssoServiceInfo = null;
        private Dictionary<string, ISSOService> _ssoServiceList = null;

        private TransformBlock<EventServiceMessage, EventSsoFrame> _serviceMsgTransformBlock;
        private TransformBlock<SocketPackage, EventServiceMessage> _socketMsgTransformBlock;

        private ActionBlock<EventSsoFrame> _ssoMsgActionBlock;
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
                //   SocketPackage -> EventServiceMessage
                _socketMsgTransformBlock = new TransformBlock<SocketPackage, EventServiceMessage>
                     (socketData => EventServiceMessage.Parse(socketData, out var fromService) ? fromService : null);

                // [Incoming] Working pipeline
                //   EventServiceMessage -> EventSsoFrame
                _serviceMsgTransformBlock = new TransformBlock<EventServiceMessage, EventSsoFrame>
                    (fromService => EventSsoFrame.Parse(fromService, out var ssoFrame) ? ssoFrame : null);

                // [Incoming] Action pipeline
                //   EventSsoFrame -> SSO Service
                _ssoMsgActionBlock = new ActionBlock<EventSsoFrame>
                (ssoFrame =>
                {
                    // Get service by sso command
                    if (_ssoServiceList.TryGetValue(ssoFrame.Command, out ISSOService service))
                    {
                        try
                        {
                            if (service.HandleInComing(ssoFrame, out var output))
                            {
                                // Post data to target service entity
                                if (output != null
                                    && _entityEventActionBlock.TryGetValue(ssoFrame.Owner.Id, out var action))
                                {
                                    action.SendAsync(output);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                });

                // [Incoming] Connect the working pipelines up
                //   ServiceMessage -> SSOMessage -> Service Entity
                _socketMsgTransformBlock.LinkTo(_serviceMsgTransformBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoFrame => ssoFrame != null);

                _serviceMsgTransformBlock.LinkTo(_ssoMsgActionBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoFrame => ssoFrame != null);

                // [OutGoing] Action pipeline
                //   SSO Service -> Socket
                _eventActionBlock = new ActionBlock<KonataEventArgs>
                (eventMessage =>
                {
                    // Get service by sso command
                    if (_ssoServiceList.TryGetValue(eventMessage.EventName, out ISSOService service))
                    {
                        // Serialize the packet
                        if (service.HandleOutGoing(eventMessage, out var output))
                        {
                            if (output != null
                                && _entitySocketList.TryGetValue(eventMessage.Owner.Id, out ISocket socket)
                                && socket.Connected)
                            {
                                socket.Send(output);
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

        public async KonataEventArgs WaitForResponse()
        {

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

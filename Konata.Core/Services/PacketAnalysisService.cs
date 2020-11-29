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
    class PacketAnalysisService : ILoad, IDisposable
    {
        private Dictionary<string, IPacketWorker> _packetworkerList = null;
        private List<PacketAttribute> _packetworkerInfo = null;

        private TransformBlock<SocketPackage, ServiceMessage> _socketMsgTransformBlock;

        private TransformBlock<ServiceMessage, SSOMessage> _serviceMsgTransformBlock;

        private ActionBlock<SSOMessage> _ssoMsgActionBlock;

        private ActionBlock<KonataEventArgs> _eventActionBlock;

        private ConcurrentDictionary<long, ActionBlock<KonataEventArgs>> _entityEventActionBlock;
        private ConcurrentDictionary<long, ISocket> _entitySocketList = null;

        public void Load()
        {
            //装载解析集
            if (_packetworkerList == null)
            {
                _packetworkerList = new Dictionary<string, IPacketWorker>();
                _entitySocketList = new ConcurrentDictionary<long, ISocket>();
                _packetworkerInfo = new List<PacketAttribute>();
                _entityEventActionBlock = new ConcurrentDictionary<long, ActionBlock<KonataEventArgs>>();

                foreach (Type type in typeof(PacketAnalysisService).Assembly.GetTypes())
                {
                    PacketAttribute _attr = (PacketAttribute)type.GetCustomAttributes(typeof(PacketAttribute)).FirstOrDefault();
                    if (_attr == null)
                        continue;
                    if (typeof(IPacketWorker).IsAssignableFrom(type))
                    {
                        IPacketWorker obj = (IPacketWorker)Activator.CreateInstance(type);
                        _packetworkerList.Add(_attr.PacketName, obj);
                        _packetworkerInfo.Add(_attr);
                    }
                }

                //装载完毕 初始化数据管道[Socket->Event]
                //socket->servicemsg
                _socketMsgTransformBlock = new TransformBlock<SocketPackage, ServiceMessage>(smsg =>
                {
                    //尝试将socket报文格式化为基本消息包
                    ServiceMessage msg = null;
                    if (ServiceMessage.ToServiceMessage(smsg, out msg))
                    {
                        return msg;
                    }
                    return null;
                    //最大并发线程2 每个线程最大处理队列20
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, MaxMessagesPerTask = 20 });

                //servicemsg->ssomsg
                _serviceMsgTransformBlock = new TransformBlock<ServiceMessage, SSOMessage>(smsg =>
                {
                    SSOMessage msg = null;
                    if (SSOMessage.ToSSOMessage(smsg, smsg.Payload, smsg.MsgPktType, out msg))
                    {
                        return msg;
                    }
                    return null;
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, MaxMessagesPerTask = 20 });

                //filter [ssomsg->EventArgs]->entity[ActionBlock]
                _ssoMsgActionBlock = new ActionBlock<SSOMessage>(ssoMessage =>
                {
                    //get ssocommand-PacketWorker
                    if (_packetworkerList.TryGetValue(ssoMessage.SSOCommand, out IPacketWorker worker))
                    {
                        //worker DeSerialize
                        if (worker.DeSerialize(ssoMessage, out var arg))
                        {
                            //post data to target entity
                            if (_entityEventActionBlock.TryGetValue(ssoMessage.Receiver.Id, out var action))
                            {
                                action.SendAsync(arg);
                            }
                        }
                    }
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2, MaxMessagesPerTask = 20 });

                //Finish Basic ,Link Them
                _socketMsgTransformBlock.LinkTo(_serviceMsgTransformBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoMessage => ssoMessage != null);

                _serviceMsgTransformBlock.LinkTo(_ssoMsgActionBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoMessage => ssoMessage != null);

                //初始化数据管道[Event->Socket]
                _eventActionBlock = new ActionBlock<KonataEventArgs>(eventMessage =>
                {
                    byte[] data = null;

                    //get ssocommand-PacketWorker
                    if (_packetworkerList.TryGetValue(eventMessage.EventName, out IPacketWorker worker))
                    {
                        //worker DeSerialize
                        if (worker.Serialize(eventMessage, out data))
                        {
                            if (data != null)
                            {
                                if (_entitySocketList.TryGetValue(eventMessage.Receiver.Id, out ISocket socket) && socket.Connected)
                                {
                                    socket.Send(data);
                                }
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
                    CancellationTokenSource source = new CancellationTokenSource(timeoutMs);
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
            _packetworkerList.Clear();
            _packetworkerList = null;

            foreach (var data in _entityEventActionBlock.Values)
            {
                data.Complete();
            }
            _entityEventActionBlock.Clear();
            _entityEventActionBlock = null;
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Konata.Core.Packet;
using Konata.Core.Event;
using Konata.Runtime;
using Konata.Runtime.Base;
using Konata.Runtime.Base.Event;
using Konata.Runtime.Network;

namespace Konata.Core.Service
{
    [Service("消息解析服务", "全局消息解析服务，用于消息序列化/反序列化")]
    class PacketService : ILoad, IDisposable
    {
        private List<SSOServiceAttribute> _ssoServiceInfo;
        private Dictionary<string, ISSOService> _ssoServiceList;

        private TransformBlock<EventServiceMessage, EventSsoFrame> _serviceMsgTransformBlock;
        private TransformBlock<SocketPackage, EventServiceMessage> _socketMsgTransformBlock;

        private ActionBlock<EventSsoFrame> _ssoMsgActionBlock;
        private ActionBlock<KonataEventArgs> _eventActionBlock;

        /// <summary>
        /// Service Onload
        /// </summary>
        public void Load()
        {
            if (_ssoServiceList == null)
            {
                _ssoServiceInfo = new List<SSOServiceAttribute>();
                _ssoServiceList = new Dictionary<string, ISSOService>();

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

                #region Socket->Event Method Set
                // [Incoming] Working pipeline
                //   SocketPackage -> EventServiceMessage
                _socketMsgTransformBlock = new TransformBlock<SocketPackage, EventServiceMessage>(socketData => {
                    if(EventServiceMessage.Parse(socketData, out var fromService))
                    {
                        if (fromService != null)
                        {
                            if (fromService.IsServerResponse)
                            {
                                fromService.CoreEventType = CoreEventType.TaskComplate;
                                EventManager.Instance.SendEventToEntity(fromService.Owner, fromService);
                            }
                            else
                            {
                                return fromService;
                            }
                            
                        }
                    }
                    return null;
                });

                // [Incoming] Working pipeline
                //   EventServiceMessage -> EventSsoFrame
                _serviceMsgTransformBlock = new TransformBlock<EventServiceMessage, EventSsoFrame>
                    (fromService => {
                        if(EventSsoFrame.Parse(fromService,out var ssoFrame))
                        {
                            if (ssoFrame != null)
                            {
                                if (ssoFrame.IsServerResponse)
                                {
                                    ssoFrame.CoreEventType = CoreEventType.TaskComplate;
                                    EventManager.Instance.SendEventToEntity(ssoFrame.Owner, ssoFrame);
                                }
                                else
                                {
                                    return ssoFrame;
                                }

                            }
                        }
                        return null;
                    });

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
                                if (output != null)
                                {
                                    EventManager.Instance.SendEventToEntity(ssoFrame.Owner, output);
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
                #endregion

                #region LinkPipe
                // [Incoming] Connect the working pipelines up
                //   ServiceMessage -> SSOMessage -> Service Entity
                _socketMsgTransformBlock.LinkTo(_serviceMsgTransformBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoFrame => ssoFrame != null);

                _serviceMsgTransformBlock.LinkTo(_ssoMsgActionBlock,
                    new DataflowLinkOptions { PropagateCompletion = true }, ssoFrame => ssoFrame != null);
                #endregion
                
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
                            if (output != null)
                            {
                                ServiceManager.Instance
                                    .GetService<SocketService>()
                                    .SendData(eventMessage.Owner, output);
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 直接发送socket包
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
        /// 将事件发送去服务器
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
        /// 将需要等待回复的事件发送去服务器
        /// <para>ssoreq限制,本质同步</para>
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TaskCompletionSource<KonataEventArgs> SendDataToServer(KonataEventArgs eventArgs,CancellationToken token)
        {
            if (eventArgs.Owner != null)
            {
                var com=eventArgs.Owner.GetComponent<EventComponent>();
                var callbacksource=com?.RegisterSyncTaskSource(eventArgs.EventName,()=> { this.SendDataToServer(eventArgs);},token);
                return callbacksource;
            }
            return null;
        }

        public void Dispose()
        {
            //socket-servicemsg-ssomsg linked with PropagateCompletion
            //Source Link Complete signal will send to it target
            _socketMsgTransformBlock.Complete();
            _ssoServiceList.Clear();
            _ssoServiceList = null;
        }
    }
}

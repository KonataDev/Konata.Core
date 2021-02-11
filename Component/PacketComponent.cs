using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Konata.Utils;
using Konata.Utils.IO;
using Konata.Core.Event;
using Konata.Core.Entity;
using Konata.Core.Service;
using Konata.Core.Packet;

namespace Konata.Core.Component
{
    [Component("PacketComponent", "Konata Packet Translation Component")]
    public class PacketComponent : BaseComponent
    {
        private const string TAG = "PacketComponent";

        private Dictionary<string, IService> _services;
        private Dictionary<Type, IService> _servicesType;
        private Dictionary<Type, List<IService>> _servicesEventType;

        private ConcurrentDictionary<int, TaskCompletionSource<BaseEvent>> _pendingRequests;

        private Sequence _serviceSequence;

        public PacketComponent()
        {
            _serviceSequence = new Sequence();
            _services = new Dictionary<string, IService>();
            _servicesType = new Dictionary<Type, IService>();
            _servicesEventType = new Dictionary<Type, List<IService>>();
            _pendingRequests = new ConcurrentDictionary<int, TaskCompletionSource<BaseEvent>>();

            LoadService();
        }

        /// <summary>
        /// Load sso service
        /// </summary>
        private void LoadService()
        {
            // Initialize protocol event types
            foreach (var type in Reflection.GetChildClasses<ProtocolEvent>())
            {
                _servicesEventType.Add(type, new List<IService>());
            }

            // Create sso services
            foreach (var type in Reflection.GetClassesByAttribute<ServiceAttribute>())
            {
                var eventAttrs = type.GetCustomAttributes<EventDependsAttribute>();
                var serviceAttr = type.GetCustomAttribute<ServiceAttribute>();

                if (serviceAttr != null)
                {
                    var service = (IService)Activator.CreateInstance(type);

                    // Bind service name with service
                    _services.Add(serviceAttr.ServiceName, service);
                    _servicesType.Add(type, service);

                    // Bind protocol event type with service
                    foreach (var attr in eventAttrs)
                    {
                        _servicesEventType[attr.Event].Add(service);
                    }
                }
            }
        }

        internal override void EventHandler(KonataTask task)
        {
            var config = GetComponent<ConfigComponent>();

            if (task.EventPayload is PacketEvent packetEvent)
            {
                // Parse service message
                if (ServiceMessage.Parse(packetEvent.Buffer, config.SignInfo, out var serviceMsg))
                {
                    // Parse SSO frame
                    if (SSOFrame.Parse(serviceMsg, out var ssoFrame))
                    {
                        LogV(TAG, ssoFrame.Command);

                        // Get SSO service by sso command
                        if (_services.TryGetValue(ssoFrame.Command, out var service))
                        {
                            try
                            {
                                // Translate bytes to ProtocolEvent 
                                if (service.Parse(ssoFrame, config.SignInfo, out var outEvent))
                                {
                                    if (outEvent != null)
                                    {
                                        // Get pending request
                                        if (_pendingRequests.TryRemove(ssoFrame.Sequence, out var request))
                                        {
                                            // Set result
                                            request.SetResult(outEvent);
                                        }
                                        else
                                        {
                                            // Pass this message to business
                                            PostEvent<BusinessComponent>(outEvent);
                                        }
                                    }
                                }
                                else LogW(TAG, $"This message cannot be processed. { ssoFrame.Command }");
                            }
                            catch (Exception e)
                            {
                                LogW(TAG, $"Thrown an exception while processing a message. { ssoFrame.Command }");
                                LogE(TAG, e);
                            }
                        }
                        else LogW(TAG, $"Unsupported sso frame received. { ssoFrame.Command }");
                    }
                    else LogW(TAG, $"Parse sso frame failed. { ssoFrame.Command }");
                }
                else LogW(TAG, $"Parse service message failed. \n D2 => { ByteConverter.Hex(config.SignInfo.D2Key, true) }");
            }

            // Protocol Event
            else if (task.EventPayload is ProtocolEvent protocolEvent)
            {
                // If no service supported this message
                if (!_servicesEventType.TryGetValue(protocolEvent.GetType(), out var serviceList))
                {
                    // Drop it
                    task.CompletionSource.SetResult(null);
                    return;
                }

                // Enumerate all of the service then make binary packet
                foreach (var service in serviceList)
                {
                    if (service.Build(_serviceSequence, protocolEvent, config.SignInfo, out var sequence, out var buffer))
                    {
                        // Pass messages to socket
                        PostEvent<SocketComponent>(new PacketEvent
                        {
                            Buffer = buffer,
                            EventType = PacketEvent.Type.Send
                        });

                        // Is need response from server
                        if (protocolEvent.WaitForResponse)
                        {
                        AddPending:
                            if (!_pendingRequests.TryAdd(sequence, task.CompletionSource))
                            {
                                _pendingRequests[sequence].SetCanceled();
                                _pendingRequests.TryRemove(sequence, out _);

                                // Try it again
                                goto AddPending;
                            }
                        }
                    }
                }
            }

            // Unsupported event
            else
            {
                LogW(TAG, "Unsupported Event received?");
            }
        }
    }
}

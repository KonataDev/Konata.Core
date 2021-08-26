using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Konata.Core.Utils;
using Konata.Core.Events;
using Konata.Core.Entity;
using Konata.Core.Services;
using Konata.Core.Packets;
using Konata.Core.Attributes;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Model
{
    [Component("PacketComponent", "Konata Packet Translation Component")]
    internal class PacketComponent : InternalComponent
    {
        private const string TAG = "PacketComponent";

        private readonly Dictionary<string, IService> _services;
        private readonly Dictionary<Type, IService> _servicesType;
        private readonly Dictionary<Type, List<IService>> _servicesEventType;
        private readonly ConcurrentDictionary<int, KonataTask> _pendingRequests;
        private readonly Sequence _serviceSequence;

        public PacketComponent()
        {
            _serviceSequence = new();
            _services = new();
            _servicesType = new();
            _servicesEventType = new();
            _pendingRequests = new();

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
                var eventAttrs = type.GetCustomAttributes<EventSubscribeAttribute>();
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
                if (ServiceMessage.Parse(packetEvent.Buffer, config.KeyStore, out var serviceMsg))
                {
                    // Parse SSO frame
                    if (SSOFrame.Parse(serviceMsg, out var ssoFrame))
                    {
                        LogV(TAG, ssoFrame.Command);

                        // Get SSO service by sso command
                        if (_services.TryGetValue(ssoFrame.Command, out var service))
                        {
                            // Take pending request
                            var isPending = _pendingRequests
                                .TryRemove(ssoFrame.Sequence, out var request);

                            try
                            {
                                // Translate bytes to ProtocolEvent 
                                if (service.Parse(ssoFrame, config.KeyStore, out var outEvent))
                                {
                                    if (outEvent != null)
                                    {
                                        if (isPending)
                                        {
                                            // Set result
                                            request.Finish(outEvent);
                                        }
                                        else
                                        {
                                            // Pass this message to business
                                            PostEvent<BusinessComponent>(outEvent);
                                        }
                                    }
                                }
                                else
                                {
                                    request.Exception(new Exception("Cannot be processed."));
                                    LogW(TAG, $"This message cannot be processed. {ssoFrame.Command}");
                                }
                            }
                            catch (Exception e)
                            {
                                if (isPending)
                                {
                                    request.Exception(e);
                                }

                                task.Exception(e);
                                LogW(TAG, $"Thrown an exception while processing a message. {ssoFrame.Command}");
                                LogE(TAG, e);
                            }
                        }
                        else LogW(TAG, $"Unsupported sso frame received. {ssoFrame.Command}");
                    }
                    else LogW(TAG, $"Parse sso frame failed. {ssoFrame.Command}");
                }
                else LogW(TAG, "Parse service message failed.");
            }

            // Protocol Event
            else if (task.EventPayload is ProtocolEvent protocolEvent)
            {
                // If no service supported this message
                if (!_servicesEventType.TryGetValue(protocolEvent.GetType(), out var serviceList))
                {
                    // Drop it
                    task.Cancel();
                    return;
                }

                // Enumerate all of the service then make binary packet
                foreach (var service in serviceList)
                {
                    if (service.Build(_serviceSequence, protocolEvent,
                        config.KeyStore, config.DeviceInfo, out var sequence, out var buffer))
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
                            if (!_pendingRequests.TryAdd(sequence, task))
                            {
                                _pendingRequests[sequence].Cancel();
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
                task.Cancel();
                LogW(TAG, "Unsupported Event received?");
            }
        }
    }
}

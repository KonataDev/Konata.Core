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

// ReSharper disable InvertIf
// ReSharper disable UnusedParameter.Local
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
                    var service = (IService) Activator.CreateInstance(type);

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

        /// <summary>
        /// On handle event
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        internal override bool OnHandleEvent(KonataTask task)
        {
            // Incoming and outgoing
            switch (task.EventPayload)
            {
                // Packet Event
                case PacketEvent incoming:
                    return OnIncoming(task, incoming);

                // Protocol Event
                case ProtocolEvent outgoing:
                    return OnOutgoing(task, outgoing);
            }

            // Unsupported event
            LogW(TAG, "Unsupported event received?");
            return false;
        }

        private bool OnIncoming(KonataTask task, PacketEvent packetEvent)
        {
            // Parse service message
            if (!ServiceMessage.Parse(packetEvent.Buffer,
                ConfigComponent.KeyStore, out var serviceMsg))
            {
                LogW(TAG, "Parse message failed.");
                return false;
            }

            //  Parse sso frame
            if (!SSOFrame.Parse(serviceMsg, out var ssoFrame))
            {
                LogW(TAG, $"Unsupported sso frame received. {ssoFrame.Command}");
                return false;
            }

            // Get sso service by sso command
            LogV(TAG, ssoFrame.Command);
            if (!_services.TryGetValue(ssoFrame.Command, out var service))
            {
                LogW(TAG, $"Unsupported sso frame received. {ssoFrame.Command}");
                return false;
            }

            // Take pending request
            var isPending = _pendingRequests
                .TryRemove(ssoFrame.Sequence, out var request);

            try
            {
                // Translate bytes to ProtocolEvent 
                if (service.Parse(ssoFrame, ConfigComponent.KeyStore,
                    out var outEvent) && outEvent != null)
                {
                    // Set result
                    if (isPending) request.Finish(outEvent);

                    // Pass this message to business
                    else PostEvent<BusinessComponent>(outEvent);
                }
                else
                {
                    LogW(TAG, $"This message cannot be processed. {ssoFrame.Command}");
                }
            }
            catch (Exception e)
            {
                // Throw exception back
                if (isPending) request.Exception(e);

                LogW(TAG, $"Thrown an exception while " +
                          $"processing a message. {ssoFrame.Command}");
                LogE(TAG, e);
            }

            return false;
        }

        [Obsolete("Need to refactor")]
        private bool OnOutgoing(KonataTask task, ProtocolEvent protocolEvent)
        {
            // If no service can process this message
            if (!_servicesEventType.TryGetValue
                (protocolEvent.GetType(), out var serviceList))
                return false;

            // Enumerate all the service
            // for outgoing packet building 
            foreach (var service in serviceList)
            {
                if (service.Build(_serviceSequence, protocolEvent, ConfigComponent.KeyStore,
                    ConfigComponent.DeviceInfo, out var sequence, out var buffer))
                {
                    // Pass messages to socket
                    PostEvent<SocketComponent>(PacketEvent.Create(buffer));

                    // This event is no need response
                    if (!protocolEvent.WaitForResponse) continue;

                    // Add pending task 
                    if (!_pendingRequests.TryAdd(sequence, task))
                    {
                        _pendingRequests[sequence].Cancel();
                        _pendingRequests.TryRemove(sequence, out _);

                        // Try it again
                        if (!_pendingRequests.TryAdd(sequence, task))
                            throw new Exception("This sequence is busy");
                    }
                }
            }

            return true;
        }
    }
}

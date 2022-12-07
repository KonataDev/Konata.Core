using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Konata.Core.Utils;
using Konata.Core.Events;
using Konata.Core.Entity;
using Konata.Core.Components.Services;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Utils.Extensions;

// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Components;

[Component("PacketComponent", "Konata Packet Translation Component")]
internal class PacketComponent : InternalComponent
{
    private const string TAG = "PacketComponent";

    private readonly Sequence _serviceSequence;
    private readonly Dictionary<string, IService> _services;
    private readonly ConcurrentDictionary<int, KonataTask> _pendingRequests;
    private readonly Dictionary<Type, List<(ServiceAttribute Attr, IService Instance)>> _servicesEventType;

    public PacketComponent()
    {
        _services = new();
        _serviceSequence = new();
        _servicesEventType = new();
        _pendingRequests = new();

        LoadServices();
    }

    /// <summary>
    /// Load sso service
    /// </summary>
    private void LoadServices()
    {
        // Initialize protocol event types
        foreach (var type in Reflection.GetChildClasses<ProtocolEvent>())
            _servicesEventType.Add(type, new());

        // Create sso services
        foreach (var type in Reflection.GetClassesByAttribute<ServiceAttribute>())
        {
            var eventAttrs = type.GetCustomAttributes<EventSubscribeAttribute>();
            var serviceAttr = type.GetCustomAttribute<ServiceAttribute>();

            if (serviceAttr != null)
            {
                var service = (IService) Activator.CreateInstance(type);

                // Bind service name with service
                _services.Add(serviceAttr.Command, service);

                // Bind protocol event type with service
                foreach (var attr in eventAttrs)
                {
                    _servicesEventType[attr.Event].Add((serviceAttr, service));
                }
            }
        }
    }

    /// <summary>
    /// On handle event
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public override Task<bool> OnHandleEvent(KonataTask task)
    {
        // Incoming and outgoing
        switch (task.EventPayload)
        {
            // Packet Event
            case PacketEvent incoming:
                return Task.FromResult(OnIncoming(task, incoming));

            // Protocol Event
            case ProtocolEvent outgoing:
                return Task.FromResult(OnOutgoing(task, outgoing));
        }

        // Unsupported event
        LogW(TAG, "Unsupported event received?");
        return Task.FromResult(false);
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
            LogW(TAG, $"parse sso frame failed. {ssoFrame.Command}");
            return false;
        }

        // Get sso service by sso command
        LogV(TAG, $"[recv:{ssoFrame.Command}] \n{ssoFrame.Payload.GetBytes().ToHex()}");
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
            if (service.Parse(ssoFrame, ConfigComponent.AppInfo, ConfigComponent.KeyStore,
                    out var outEvent, out var outExtra) && outEvent != null)
            {
                // Set result
                if (isPending) request.Finish(outEvent);

                // Pass this message to business
                else PushBusiness(BusinessComponent, outEvent);

                // Pass extra messages to business
                PushBusiness(BusinessComponent, outExtra);
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

    private bool OnOutgoing(KonataTask task, ProtocolEvent protocolEvent)
    {
        // If no service can process this message
        if (!_servicesEventType.TryGetValue
                (protocolEvent.GetType(), out var serviceList))
            return false;

        // Enumerate all the service
        // for outgoing packet building 
        foreach (var (attr, instance) in serviceList)
        {
            // Allocate a new sequence
            var sequence = attr.SeqMode switch
            {
                SequenceMode.Session => _serviceSequence.GetSessionSequence(attr.Command),
                SequenceMode.Managed => _serviceSequence.GetNewSequence(),
                SequenceMode.EventBased => protocolEvent.SessionSequence,
                _ => throw new NotSupportedException()
            };

            var wupBuffer = new PacketBase();

            // Build body data
            var result = instance.Build(sequence, protocolEvent, ConfigComponent.AppInfo,
                ConfigComponent.KeyStore, ConfigComponent.DeviceInfo, ref wupBuffer);
            {
                if (!result) continue;
                LogV(TAG, $"[send:{attr.Command}] \n{wupBuffer.GetBytes().ToHex()}");

                // Build sso frame
                if (!SSOFrame.Create(attr.Command, attr.PacketType, sequence,
                        attr.NeedTgtToken ? ConfigComponent.KeyStore.Session.TgtToken : null,
                        _serviceSequence.Session, wupBuffer, out var ssoFrame))
                    throw new Exception("Create sso frame failed.");

                // Build to service message
                if (!ServiceMessage.Create(ssoFrame, attr.AuthType, Bot.Uin,
                        ConfigComponent.KeyStore.Session.D2Token,
                        ConfigComponent.KeyStore.Session.D2Key, out var toService))
                    throw new Exception("Create service message failed.");

                // Pack up
                if (!ServiceMessage.Build(toService,
                        ConfigComponent.AppInfo, ConfigComponent.DeviceInfo, out var output))
                    throw new Exception("Build packet failed");

                // Pass messages to socket
                PostEvent<SocketComponent>(PacketEvent.Create(output));
                {
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
        }

        return true;
    }

    #region Stub methods

    private static void PushBusiness(BusinessComponent context, List<ProtocolEvent> anyEvent)
    {
        foreach (var i in anyEvent)
            context.PostEvent<BusinessComponent>(i);
    }

    private static void PushBusiness(BusinessComponent context, ProtocolEvent anyEvent)
        => context.PostEvent<BusinessComponent>(anyEvent);

    #endregion
}

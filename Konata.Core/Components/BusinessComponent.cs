using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Components.Logics;
using Konata.Core.Components.Logics.Model;
using Konata.Core.Entity;
using Konata.Core.Events;
using Konata.Core.Utils;

// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components;

[Component("BusinessComponent", "Konata Business Component")]
internal class BusinessComponent : InternalComponent
{
    private const string TAG = "BusinessComponent";
    private readonly Dictionary<Type, List<BaseLogic>> _businessLogics;
    private int _taskTimeout;

    public BusinessComponent()
    {
        _taskTimeout = 0;
        _businessLogics = new();
    }

    public override void OnLoad()
    {
        // Load all business logics
        Reflection.EnumAttributes<BusinessLogicAttribute>((type, _) =>
        {
            // Event to subscribe 
            var events = type.GetCustomAttributes<EventSubscribeAttribute>();

            // Logic instance
            var constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var instance = (BaseLogic) constructor[0].Invoke(new object[] {this});

            // Bind logic withevents
            foreach (var i in events)
            {
                // Create the key
                if (!_businessLogics.TryGetValue(i.Event, out var list))
                {
                    list = new();
                    _businessLogics.Add(i.Event, list);
                }

                // Append logics
                list.Add(instance);
            }

            // Save the cache
            switch (instance)
            {
                case MessagingLogic messaging:
                    Messaging = messaging;
                    break;

                case OperationLogic operation:
                    Operation = operation;
                    break;

                case WtExchangeLogic wtxchg:
                    WtExchange = wtxchg;
                    break;

                case CacheSyncLogic cache:
                    CacheSync = cache;
                    break;

                case PushEventLogic pushEv:
                    PushEvent = pushEv;
                    break;
            }
        });
    }

    public override void OnStart()
    {
        _taskTimeout = ConfigComponent.GlobalConfig.DefaultTimeout;

        if (_taskTimeout <= 2000)
        {
            LogW(TAG, "The timeout you configured is less than 2000ms, " +
                      "this can cause server communication chances to fail. Force reseted to 6000ms.");
            _taskTimeout = 6000;
        }
    }

    public override void OnDestroy()
    {
    }

    /// <summary>
    /// Business logics
    /// </summary>
    /// <param name="task"></param>
    public override async Task<bool> OnHandleEvent(KonataTask task)
    {
        // Pass if not a protocol event
        if (task.EventPayload is not ProtocolEvent protocolEvent) return false;

        // Get logics
        _businessLogics.TryGetValue
            (typeof(ProtocolEvent), out var baseLogics);

        // Handle event
        if (_businessLogics.TryGetValue
                (protocolEvent.GetType(), out var logics))
        {
            // Append base logics and
            // select distinct to avoid multiple executes
            if (baseLogics != null)
            {
                logics.AddRange(baseLogics);
                logics = logics.Distinct().ToList();
            }

            foreach (var i in logics)
            {
                try
                {
                    // Execute a business logic
                    await i.Incoming(protocolEvent);
                }
                catch (Exception e)
                {
                    LogE(TAG, $"The logic '{i.GetType()}'" +
                              " was thrown an exception:");
                    LogE(TAG, e);
                }
            }
        }

        // No handler
        else
        {
            LogW(TAG, "The event has no logic to handle.");
        }

        return false;
    }

    #region Business Logics

    internal WtExchangeLogic WtExchange { get; private set; }

    internal OperationLogic Operation { get; private set; }

    internal MessagingLogic Messaging { get; private set; }

    internal CacheSyncLogic CacheSync { get; private set; }

    internal PushEventLogic PushEvent { get; private set; }

    #endregion

    #region Utils

    public Task<TEvent> SendPacket<TEvent>(ProtocolEvent anyEvent)
        where TEvent : ProtocolEvent
    {
        var handle = new TaskCompletionSource<TEvent>();
        {
            // Execute task
            var task = anyEvent.WaitForResponse
                ? Entity.SendEvent<PacketComponent>(anyEvent, _taskTimeout)
                : Entity.SendEvent<PacketComponent>(anyEvent);

            // Force to cast result type
            task.ContinueWith(t =>
            {
                if (t.IsFaulted) handle.TrySetException(t.Exception!.InnerExceptions);
                else if (t.IsCanceled) handle.TrySetCanceled();
                else handle.SetResult((TEvent) t.Result);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
        return handle.Task;
    }

    public void PostPacket(ProtocolEvent anyEvent)
        => Entity.PostEvent<PacketComponent>(anyEvent);

    #endregion
}

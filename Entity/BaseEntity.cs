﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Utils;
using Konata.Core.Events;
using Konata.Core.Components;

// ReSharper disable InvertIf
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable AsyncVoidLambda
// ReSharper disable EmptyGeneralCatchClause
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Entity;

public class BaseEntity
{
    /// <summary>
    /// Conpoments on this entity
    /// </summary>
    private readonly Dictionary<Type, BaseComponent> _componentDict;

    public BaseEntity()
    {
        _componentDict = new();
    }

    /// <summary>
    /// Load components
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    public void LoadComponents<TAttribute>() where TAttribute : Attribute
    {
        foreach (var type in Reflection.GetClassesByAttribute(typeof(TAttribute)))
        {
            AddComponent((BaseComponent) Activator.CreateInstance(type));
        }
    }

    /// <summary>
    /// Load components with a filter
    /// </summary>
    /// <param name="filter"></param>
    /// <typeparam name="TAttribute"></typeparam>
    public void LoadComponents<TAttribute>(Func<TAttribute, bool> filter)
        where TAttribute : Attribute
    {
        foreach (var type in Reflection.GetClassesByAttribute(typeof(TAttribute)))
        {
            if (filter.Invoke((TAttribute) type.GetCustomAttribute(typeof(TAttribute))))
            {
                AddComponent((BaseComponent) Activator.CreateInstance(type));
            }
        }
    }

    /// <summary>
    /// Unload all components
    /// </summary>
    /// <returns></returns>
    public void UnloadComponents()
    {
        // Destroy components
        foreach (var comp in _componentDict) comp.Value.OnDestroy();

        // Clear
        _componentDict.Clear();
    }

    /// <summary>
    /// Get component which attached on this entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetComponent<T>() where T : BaseComponent
    {
        if (!_componentDict.TryGetValue(typeof(T), out var component)) return default;
        return (T) component;
    }

    /// <summary>
    /// Add component to this entity
    /// </summary>
    /// <param name="component"></param>
    public void AddComponent(BaseComponent component)
    {
        if (_componentDict.TryGetValue(component.GetType(), out _))
        {
            _componentDict[component.GetType()] = component;
            return;
        }

        component.Entity = this;
        _componentDict.Add(component.GetType(), component);
    }

    /// <summary>
    /// Delete component
    /// </summary>
    /// <param name="type"></param>
    public void RemoveComponent(Type type)
    {
        if (!_componentDict.TryGetValue(type, out _)) return;

        _componentDict[type].OnDestroy();
        _componentDict.Remove(type);
    }

    /// <summary>
    /// Post an event to entity
    /// </summary>
    /// <param name="anyEvent"></param>
    public virtual void PostEventToEntity(BaseEvent anyEvent)
    {
    }

    /// <summary>
    /// Send an event to a component whitch
    /// attached under this entity with timeout
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <param name="timeout"></param>
    /// <typeparam name="TComponent"></typeparam>
    /// <returns></returns>
    public Task<BaseEvent> SendEvent<TComponent>(BaseEvent anyEvent, int timeout = 0)
        where TComponent : BaseComponent
    {
        // Create a task
        var task = new KonataTask(anyEvent, timeout);
        var compoment = GetComponent<TComponent>();

        ThreadPool.QueueUserWorkItem(async _ =>
        {
            try
            {
                // Force finish the tasks if the
                // handler does not save the event by itself.
                if (!await compoment.OnHandleEvent(task) && !task.Complected)
                    task.Finish();

                // TODO:
                // Task unfinished?
            }
            catch (Exception e)
            {
                if (!task.Complected) task.Exception(e);
            }
        });

        return task.CompletionSource.Task;
    }

    /// <summary>
    /// Post an event to a component whitch
    /// attached under this entity
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <typeparam name="TComponent"></typeparam>
    public void PostEvent<TComponent>(BaseEvent anyEvent)
        where TComponent : BaseComponent
    {
        // Create a task
        var task = new KonataTask(anyEvent);
        var compoment = GetComponent<TComponent>();

        ThreadPool.QueueUserWorkItem(async _ =>
        {
            try
            {
                // Force finish the tasks if the
                // handler does not save the event by itself.
                if (!await compoment.OnHandleEvent(task) && !task.Complected)
                    task.Finish();

                // TODO:
                // Task unfinished?
            }
            catch (Exception)
            {
            }
        });
    }

    /// <summary>
    /// Broad an event to all components
    /// </summary>
    /// <param name="anyEvent"></param>
    public void BroadcastEvent(BaseEvent anyEvent)
    {
        foreach (var component in _componentDict)
        {
            component.Value.OnHandleEvent(new KonataTask(anyEvent));
        }
    }
}

public class KonataTask
{
    public BaseEvent EventPayload { get; }

    public bool Complected
        => CompletionSource.Task.IsCompleted;

    public TaskCompletionSource<BaseEvent> CompletionSource { get; }

    private CancellationTokenSource CancellationToken { get; }

    public KonataTask(BaseEvent e, int timeout = 0)
    {
        EventPayload = e;
        CompletionSource = new TaskCompletionSource<BaseEvent>();

        if (timeout > 0)
        {
            CancellationToken = new(timeout);
            CancellationToken.Token.Register(() =>
                Cancel(new TimeoutException("Don't be a pigeon. =(:3)z)_")));
        }
    }

    ~KonataTask()
    {
        CancellationToken?.Dispose();
    }

    internal void Finish(BaseEvent e)
        => CompletionSource.SetResult(e);

    internal void Finish()
    {
        CompletionSource.SetResult(null);
        CancellationToken?.Cancel();
    }

    internal void Exception(Exception e)
    {
        CompletionSource.SetException(e);
        CancellationToken?.Cancel();
    }

    internal void Cancel()
    {
        CompletionSource.SetCanceled();
        CancellationToken?.Cancel();
    }

    internal void Cancel(TimeoutException e)
    {
        if (!CompletionSource.Task.IsCompleted)
        {
            Exception(e);
        }
    }
}

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using Konata.Core.Utils;
using Konata.Core.Events;
using Konata.Core.Components;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Entity
{
    public class BaseEntity
    {
        private readonly Dictionary<Type, BaseComponent> _componentDict = new();

        /// <summary>
        /// Load components
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        public void LoadComponents<TAttribute>()
            where TAttribute : Attribute
        {
            foreach (var type in Reflection
                .GetClassesByAttribute(typeof(TAttribute)))
            {
                AddComponent((BaseComponent) Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// Load components with filter
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="TAttribute"></typeparam>
        public void LoadComponents<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : Attribute
        {
            foreach (var type in Reflection
                .GetClassesByAttribute(typeof(TAttribute)))
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
            // TODO: destroy components
            _componentDict.Clear();
        }

        /// <summary>
        /// Get component which attached on this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>()
            where T : BaseComponent
        {
            if (!_componentDict.TryGetValue(typeof(T),
                out BaseComponent component))
            {
                return default(T);
            }

            return (T) component;
        }

        /// <summary>
        /// Add component to this entity
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(BaseComponent component)
        {
            if (_componentDict.TryGetValue(component.GetType(), out var _))
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
            if (!_componentDict.TryGetValue
                (type, out BaseComponent _))
            {
                return;
            }

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
        /// Post an event to any component attached under this entity
        /// </summary>
        /// <param name="anyEvent"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public Task<BaseEvent> PostEvent<TComponent>(BaseEvent anyEvent)
            where TComponent : BaseComponent
        {
            var task = new KonataTask(anyEvent);
            {
                GetComponent<TComponent>().EventPipeline.SendAsync(task);
            }

            return task.CompletionSource.Task;
        }

        /// <summary>
        /// Broad an event to all components
        /// </summary>
        /// <param name="anyEvent"></param>
        public void BroadcastEvent(BaseEvent anyEvent)
        {
            foreach (var component in _componentDict)
            {
                component.Value.EventPipeline.SendAsync(new KonataTask(anyEvent));
            }
        }

        private static Task RunAsync(Action action)
        {
            var taskSource = new TaskCompletionSource<Object>();

            ThreadPool.QueueUserWorkItem((_) =>
            {
                try
                {
                    action();
                    taskSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
            });

            return taskSource.Task;
        }
    }

    public class KonataTask
    {
        public BaseEvent EventPayload { get; }

        public TaskCompletionSource<BaseEvent> CompletionSource { get; }

        public KonataTask(BaseEvent e)
        {
            EventPayload = e;
            CompletionSource = new TaskCompletionSource<BaseEvent>();
        }
    }
}

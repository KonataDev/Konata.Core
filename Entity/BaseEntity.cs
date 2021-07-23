using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

using Konata.Utils;
using Konata.Core.Events;
using Konata.Core.Components;
using System.Reflection;

namespace Konata.Core.Entity
{
    public class BaseEntity
    {
        private Dictionary<Type, BaseComponent> _componentDict = new();

        /// <summary>
        /// Load components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void LoadComponents<T>()
            where T : Attribute
        {
            foreach (var type in Reflection
                .GetClassesByAttribute(typeof(T)))
            {
                AddComponent((BaseComponent)Activator.CreateInstance(type));
            }
        }

        /// Load components with filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void LoadComponents<T>(Func<T, bool> filter)
            where T : Attribute
        {
            foreach (var type in Reflection
                .GetClassesByAttribute(typeof(T)))
            {
                if (filter.Invoke((T)type.GetCustomAttribute(typeof(T))))
                {
                    AddComponent((BaseComponent)Activator.CreateInstance(type));
                }
            }
        }

        /// <summary>
        /// Unload all components
        /// </summary>
        /// <returns></returns>
        public void UnloadComponents()
        {
            // TODO
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
            if (!_componentDict.TryGetValue(typeof(T), out BaseComponent component))
            {
                return default(T);
            }
            return (T)component;
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
                (type, out BaseComponent component))
            {
                return;
            }
            _componentDict.Remove(type);
            return;
        }

        /// <summary>
        /// Post an event to entity
        /// </summary>
        /// <param name="anyEvent"></param>
        public virtual void PostEventToEntity(BaseEvent anyEvent) { }

        /// <summary>
        /// Post an event to any component attached under this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anyEvent"></param>
        /// <returns></returns>
        public Task<BaseEvent> PostEvent<T>(BaseEvent anyEvent)
            where T : BaseComponent
        {
            var task = new KonataTask(anyEvent);
            {
                GetComponent<T>().EventPipeline.SendAsync(task);
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

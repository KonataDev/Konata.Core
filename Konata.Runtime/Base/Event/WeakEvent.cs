using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Konata.Runtime.Base.Event
{
    public class WeakEvent<TSender, TArgs>
    {
        private readonly ReaderWriterLockSlim locker
            = new ReaderWriterLockSlim();

        private readonly List<WeakReference<object>> relatedInstances
            = new List<WeakReference<object>>();

        private readonly ConditionalWeakTable<object, WeakEventHandler> handlers
            = new ConditionalWeakTable<object, WeakEventHandler>();

        public void Add(MulticastDelegate original, Action<TSender, TArgs> casted)
        {
            var target = original.Target;
            var method = original.Method;

            if (target == null)
            {
                throw new ArgumentNullException("Event must an object");
            }

            locker.EnterWriteLock();
            try
            {
                var reference = relatedInstances.Find(x => x.TryGetTarget(out var instance) && ReferenceEquals(target, instance));
                if (reference == null)
                {
                    reference = new WeakReference<object>(target);
                    relatedInstances.Add(reference);
                    var weakEventHandler = new WeakEventHandler();
                    weakEventHandler.Add(original, casted);
                    handlers.Add(target, weakEventHandler);
                }
                else if (handlers.TryGetValue(target, out var weakEventHandler))
                {
                    weakEventHandler.Add(original, casted);
                }
                else
                {
                    throw new InvalidOperationException("can not be happened");
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public void Remove(MulticastDelegate original)
        {
            var target = original.Target;
            if (target == null)
            {
                return;
            }

            locker.EnterWriteLock();
            try
            {
                var reference = relatedInstances.Find(x => x.TryGetTarget(out var instance) && ReferenceEquals(target, instance));
                if (reference == null)
                {

                }
                else if (handlers.TryGetValue(target, out var weakEventHandler))
                {
                    weakEventHandler.Remove(original);
                }
                else
                {
                    throw new InvalidOperationException("can not be happened");
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public bool Invoke(TSender sender, TArgs arg)
        {
            List<Action<TSender, TArgs>> invokeHandlers = null;
            locker.EnterReadLock();
            try
            {
                var list = relatedInstances.ConvertAll(x =>
                      x.TryGetTarget(out var instance) && handlers.TryGetValue(instance, out var value)
                      ? value : null);
                var AnyAlive = list.Exists(x => x != null);
                if (AnyAlive)
                {
                    invokeHandlers = list.OfType<WeakEventHandler>().SelectMany(x => x.GetInvokeHandlers()).ToList();
                }
                else
                {
                    invokeHandlers = null;
                    relatedInstances.Clear();
                }
            }
            finally
            {
                locker.ExitReadLock();
            }
            if (invokeHandlers != null)
            {
                foreach (var handler in invokeHandlers)
                {
                    var strongHandler = handler;
                    strongHandler(sender, arg);
                }
            }

            return invokeHandlers != null;

        }

        private sealed class WeakEventHandler
        {
            internal object Target { get; private set; }
            private Dictionary<MethodInfo, List<Action<TSender, TArgs>>> methodHandlers { get; } = new Dictionary<MethodInfo, List<Action<TSender, TArgs>>>();
            internal void Add(MulticastDelegate handler, Action<TSender, TArgs> castedHandler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }
                if (Target != null && Target != handler.Target)
                {
                    throw new ArgumentException("Should not happened");
                }
                Target = handler.Target;

                if (methodHandlers.TryGetValue(handler.Method, out var handlers))
                {
                    handlers.Add(castedHandler);
                }
                else
                {
                    handlers = new List<Action<TSender, TArgs>>
                    {
                        castedHandler,
                    };
                    methodHandlers[handler.Method] = handlers;
                }
            }

            internal void Remove(MulticastDelegate handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }
                if (Target != null && Target != handler.Target)
                {
                    throw new ArgumentException("Should not happened");
                }

                if (methodHandlers.TryGetValue(handler.Method, out var handlers))
                {
                    handlers.RemoveAt(handlers.Count - 1);
                    if (handlers.Count == 0)
                    {
                        methodHandlers.Remove(handler.Method);
                    }
                }
            }
            internal IReadOnlyList<Action<TSender, TArgs>> GetInvokeHandlers()
            {
                return methodHandlers.SelectMany(x => x.Value).ToList();
            }
        }
    }

    public class WeakEvent<TArgs> : WeakEvent<object, TArgs>
    {

    }

}

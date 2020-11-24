using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Konata.Core.Base.Event
{
    public class WeakEvent<T>
        where T:KonataEventArgs
    {
        private object listlock = new object();
        private T[] tempargs = new T[1];
        private class Unit
        {
            private WeakReference reference;
            private MethodInfo method;
            private bool isStatic;

            public bool IsDead
            {
                get => !this.isStatic && !this.reference.IsAlive;
            }

            public Unit(Action<T> action)
            {
                this.isStatic = action.Target == null;
                this.reference = new WeakReference(action.Target);
                this.method = action.Method;
            }

            public bool Equals(Action<T> action)
            {
                return this.reference.Target == action.Target && this.method == action.Method;
            }

            public void Invoke(object[] args)
            {
                this.method.Invoke(this.reference.Target, args);
            }
        }

        private List<Unit> list = new List<Unit>();

        public int Count
        {
            get => this.list.Count;
        }
        public void Add(Action<T> action)
        {
            lock (listlock)
            {
                this.list.Add(new Unit(action));
            }
        }
        public void Remove(Action<T> action)
        {
            lock (listlock)
            {
                for (int i = this.list.Count - 1; i >= 0; i--)
                {
                    if (this.list[i].Equals(action))
                    {
                        this.list.RemoveAt(i);
                    }
                }
            }
        }
        public void Invoke(T args = null)
        {
            this.tempargs[0] = args;

            lock (listlock)
            {
                for (int i = this.list.Count - 1; i >= 0; i--)
                {
                    if (this.list[i].IsDead)
                    {
                        this.list.RemoveAt(i);
                    }
                    else
                    {
                        this.list[i].Invoke(this.tempargs);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (listlock)
            {
                this.list.Clear();
            }
        }

        public static WeakEvent<T> operator + (WeakEvent<T> weakevent,Action<T> action)
        {
            weakevent.Add(action);
            return weakevent;
        }
        public static WeakEvent<T> operator - (WeakEvent<T> weakevent, Action<T> action)
        {
            weakevent.Remove(action);
            return weakevent;
        }
    }
}

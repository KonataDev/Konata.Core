using System;
using System.Threading;
using System.Collections.Generic;

using Konata.Runtime.Utils;

namespace Konata.Runtime.Base
{
    /// <summary>
    /// 全局组件/实体缓存池
    /// </summary>
    public sealed class ObjectPool
    {
        private static ObjectPool instance;

        public static ObjectPool Instance
        {
            get => instance ?? (instance = new ObjectPool());
        }

        private readonly Dictionary<Type, Queue<BaseObject>> pool = new Dictionary<Type, Queue<BaseObject>>();

        private ReaderWriterLockSlim poolLock = new ReaderWriterLockSlim();
        private ObjectPool() { }

        /// <summary>
        /// 立即释放所有对象缓存
        /// </summary>
        public static void Release()
        {
            instance.DisposeAll();
            instance = null;
        }

        /// <summary>
        /// 从缓存池获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">指定对象类型[组件/实体]</typeparam>
        /// <returns></returns>
        public T Fetch<T>()
            where T : BaseObject
        {
            return (T)this.Fetch(typeof(T));
        }

        /// <summary>
        /// 从缓存池获取指定类型的对象
        /// </summary>
        /// <param name="type">目标对象类型</param>
        /// <returns></returns>
        public BaseObject Fetch(Type type)
        {
            poolLock.EnterWriteLock();
            try
            {
                if (!this.pool.TryGetValue(type, out Queue<BaseObject> queue))
                {
                    queue = new Queue<BaseObject>();
                    this.pool.Add(type, queue);
                }

                if (queue.Count > 0)
                {
                    BaseObject obj = queue.Dequeue();
                    obj.Id = IdGenerater.GenerateID();
                    return obj;
                }

                return (BaseObject)Activator.CreateInstance(type);
            }
            finally
            {
                poolLock.ExitWriteLock();
            }

        }

        /// <summary>
        /// 再利用废弃对象
        /// </summary>
        /// <param name="baseObject">被卸载的组件/实体</param>
        public void Recycle(BaseObject baseObject)
        {
            poolLock.EnterWriteLock();
            try
            {

            }
            finally
            {
                poolLock.ExitWriteLock();
            }
            Type type = baseObject.GetType();
            if (!this.pool.TryGetValue(type, out Queue<BaseObject> queue))
            {
                queue = new Queue<BaseObject>();
                this.pool.Add(type, queue);
            }
            queue.Enqueue(baseObject);
        }

        /// <summary>
        /// 从缓存池释放指定类型
        /// </summary>
        /// <param name="type"></param>
        public void DisposeType(Type type)
        {
            poolLock.EnterWriteLock();
            try
            {
                if (!this.pool.TryGetValue(type, out Queue<BaseObject> queue))
                {
                    if (queue == null || queue.Count == 0)
                    {
                        return;
                    }
                    while (queue.Count > 0)
                    {
                        BaseObject obj = queue.Dequeue();
                        obj.Dispose();
                    }
                }
            }
            finally
            {
                poolLock.ExitWriteLock();
            }


        }

        /// <summary>
        /// 从缓存池释放指定的所有类型
        /// </summary>
        /// <param name="types"></param>
        public void DisposeTypes(IList<Type> types)
        {
            foreach (Type t in types)
            {
                DisposeType(t);
            }
        }

        // 释放所有组件
        private void DisposeAll()
        {
            poolLock.EnterWriteLock();
            try
            {
                foreach (Queue<BaseObject> q in this.pool.Values)
                {
                    if (q == null || q.Count == 0)
                    {
                        continue;
                    }
                    while (q.Count > 0)
                    {
                        BaseObject obj = q.Dequeue();
                        obj.Dispose();
                    }
                }
            }
            finally
            {
                poolLock.ExitWriteLock();
            }

        }

        ~ObjectPool()
        {
            poolLock.Dispose();
            DisposeAll();
        }
    }
}

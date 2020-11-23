using Konata.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    public sealed class ObjectPool
    {
        private static ObjectPool instance;
        
        public static ObjectPool Instance
        {
            get => instance ?? (instance=new ObjectPool());
        }

        private readonly Dictionary<Type, Queue<BaseObject>> pool = new Dictionary<Type, Queue<BaseObject>>();

        private ObjectPool() { }

        public static void Release()
        {
            instance = null;
        }

        public T Fetch<T>()
            where T : BaseObject
        {
            return (T)this.Fetch(typeof(T));
        }

        public BaseObject Fetch(Type type)
        {
            if(!this.pool.TryGetValue(type,out Queue<BaseObject> queue))
            {
                queue = new Queue<BaseObject>();
                this.pool.Add(type,queue);
            }

            if (queue.Count > 0)
            {
                BaseObject obj = queue.Dequeue();
                obj.Id = IdGenerater.GenerateID();
                return obj;
            }

            return (BaseObject)Activator.CreateInstance(type);
        }

        public void Recycle(BaseObject baseObject)
        {
            Type type = GetType();
            if(!this.pool.TryGetValue(type, out Queue<BaseObject> queue))
            {
                queue = new Queue<BaseObject>();
                this.pool.Add(type, queue);
            }
            queue.Enqueue(baseObject);
        }

        ~ObjectPool()
        {
            foreach(Queue<BaseObject> q in this.pool.Values)
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
    }
}

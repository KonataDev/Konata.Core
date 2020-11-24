using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils
{
    /// <summary>
    /// 用于大量的key-list关联
    /// </summary>
    /// <typeparam name="T">key值</typeparam>
    /// <typeparam name="K">value链表项类型</typeparam>
    public class UnOrderMultiMap<T,K>
    {
        public int MaxRecycleListNum { get; private set; }

        private readonly Dictionary<T, List<K>> map = new Dictionary<T, List<K>>();

        private readonly Queue<List<K>> queue = new Queue<List<K>>();

        public UnOrderMultiMap(int MaxRecycleListNum=30)
        {
            this.MaxRecycleListNum = MaxRecycleListNum;
        }

        public Dictionary<T,List<K>> GetDictionary()
        {
            return this.map;
        }

        public int Count
        {
            get
            {
                return this.map.Count;
            }
        }

        public void Add(T t,K k)
        {
            if(!this.map.TryGetValue(t,out List<K> list))
            {
                list = this.FetchList();
                this.map[t] = list;
            }
            list.Add(k);
        }

        public bool Remove(T t,K k)
        {
            if(!this.map.TryGetValue(t,out List<K> list))
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.RecycleList(list);
                this.map.Remove(t);
            }
            return true;
        }

        public bool Remove(T t)
        {
            if (this.map.TryGetValue(t, out List<K> list)){
                this.RecycleList(list);
            }
            return this.map.Remove(t);
        }


        public IList<K> GetAll(T t)
        {
            if(this.map.TryGetValue(t,out List<K> list))
            {
                return list;
            }
            return null;
        }

        public List<K> this[T t]
        {
            get
            {
                this.map.TryGetValue(t, out List<K> list);
                return list;
            }
            set
            {
                if (this.map.ContainsKey(t))
                {
                    this.Remove(t);
                }
                this.map[t] = value;
            }
        }

        public bool Contains(T t,K k)
        {
            if(!this.map.TryGetValue(t,out List<K> list))
            {
                return false;
            }
            return list.Contains(k);
        }

        public bool ContainsKey(T t)
        {
            return this.map.ContainsKey(t);
        }


        public void Clear()
        {
            foreach(KeyValuePair<T,List<K>> keyValuePair in this.map)
            {
                this.RecycleList(keyValuePair.Value);
            }
            this.map.Clear();
        }

        #region recycle&getnew
        private List<K> FetchList()
        {
            if (this.queue.Count > 0)
            {
                List<K> list = this.queue.Dequeue();
                list.Clear();
                return list;
            }
            return new List<K>();
        }

        private void RecycleList(List<K> list)
        {
            if (this.queue.Count > this.MaxRecycleListNum)
            {
                return;
            }
            list.Clear();
            this.queue.Enqueue(list);
        }
        #endregion
    }
}

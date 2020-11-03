using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public sealed class List : IList<IObject>, IIndexable
        {
            public Type Type => Type.List;

            public BaseType BaseType => BaseType.List;

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public IObject this[int index]
            {
                get => list[index];
                set
                {
                    if (value is null)
                    {
                        Clear();
                        throw new ArgumentNullException("Invalid input item: Contains null object.");
                    }
                    if (value.BaseType > BaseType.MaxValue)
                    {
                        Clear();
                        throw new ArgumentException("Invalid input item: Unsupported JCE type.");
                    }
                    list[index] = value;
                }
            }

            public IObject this[string path]
            {
                get
                {
                    int dot = path.IndexOf('.');
                    if (dot >= 0)
                    {
                        int i = int.Parse(path.Substring(0, dot));
                        if (i < Count)
                        {
                            if (this[i] is IIndexable ind)
                            {
                                return ind[path.Substring(dot + 1)];
                            }
                            else
                            {
                                throw new InvalidCastException();
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException();
                        }
                    }
                    else
                    {
                        int i = int.Parse(path);
                        if (i < Count)
                        {
                            return this[i];
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
            }

            public List(int capacity) => list.Capacity = capacity;

            public List(IEnumerable<IObject> collection = null)
            {
                if (collection is object)
                {
                    foreach (var item in collection)
                    {
                        if (item is null)
                        {
                            throw new ArgumentNullException("Invalid input item: Contains null object.");
                        }
                        if (item.BaseType > BaseType.MaxValue)
                        {
                            throw new ArgumentException("Invalid input item: Unsupported JCE type.");
                        }
                    }
                    list.AddRange(collection);
                }
            }

            public int IndexOf(IObject item) => list.IndexOf(item);

            public void Insert(int index, IObject item)
            {
                if (item is null)
                {
                    throw new ArgumentNullException("Invalid input item: object is null.");
                }
                if (item.BaseType > BaseType.MaxValue)
                {
                    throw new ArgumentException("Invalid input item: Unsupported JCE type.");
                }
                try
                {
                    list.Insert(index, item);
                }
                catch
                {
                    throw;
                }
            }

            public void RemoveAt(int index) => list.RemoveAt(index);

            public void Add(IObject item)
            {
                if (item is null)
                {
                    throw new ArgumentNullException("Invalid input item: object is null.");
                }
                if (item.BaseType > BaseType.MaxValue)
                {
                    throw new ArgumentException("Invalid input item: Unsupported JCE type.");
                }
                list.Add(item);
            }

            public void Clear() => list.Clear();

            public bool Contains(IObject item) => list.Contains(item);

            public void CopyTo(IObject[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

            public bool Remove(IObject item) => list.Remove(item);

            public IEnumerator<IObject> GetEnumerator() => list.GetEnumerator();

            public override bool Equals(object obj) =>
                obj is List other &&
                Enumerable.SequenceEqual(list, other.list);

            public static implicit operator List<IObject>(List value) => new List<IObject>(value);

            public static implicit operator List(List<IObject> value) => new List(value);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private readonly List<IObject> list = new List<IObject>();
        }
    }
}
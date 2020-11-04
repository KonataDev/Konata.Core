using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public sealed class Map : IDictionary<IObject, IObject>, IIndexable
        {
            public Type Type => Type.Map;

            public BaseType BaseType => BaseType.Map;

            public ICollection<IObject> Keys => keys;

            public ICollection<IObject> Values => values;

            public int Count => keys.Count;

            public bool IsReadOnly => false;

            public IObject this[IObject key]
            {
                get
                {
                    int index = keys.IndexOf(key);
                    if (index >= 0)
                    {
                        return values[index];
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                set
                {
                    if (key is null || value is null)
                    {
                        throw new ArgumentNullException("Invalid input key or value: object is null.");
                    }
                    if (key.BaseType > BaseType.MaxValue || value.BaseType > BaseType.MaxValue)
                    {
                        throw new ArgumentException("Invalid input key or value: Unsupported JCE type.");
                    }
                    int index = keys.IndexOf(key);
                    if (index >= 0)
                    {
                        values[index] = value;
                    }
                    else
                    {
                        keys.Add(key);
                        values.Add(value);
                    }
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
                            path = path.Substring(dot + 1);
                            int kvdot = path.IndexOf('.');
                            if (kvdot >= 0)
                            {
                                int kv = int.Parse(path.Substring(0, kvdot));
                                path = path.Substring(kvdot + 1);
                                switch (kv)
                                {
                                case 0:
                                    if (keys[i] is IIndexable key)
                                    {
                                        return key[path];
                                    }
                                    else
                                    {
                                        throw new InvalidCastException();
                                    }
                                case 1:
                                    if (values[i] is IIndexable value)
                                    {
                                        return value[path];
                                    }
                                    else
                                    {
                                        throw new InvalidCastException();
                                    }
                                default:
                                    throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                int kv = int.Parse(path);
                                switch (kv)
                                {
                                case 0:
                                    return keys[i];
                                case 1:
                                    return values[i];
                                default:
                                    throw new ArgumentOutOfRangeException();
                                }
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
                            return new KeyValuePair(keys[i], values[i]);
                        }
                        else
                        {
                            throw new IndexOutOfRangeException();
                        }
                    }
                }
            }

            public Map(IDictionary<IObject, IObject> map = null)
            {
                if (map is object)
                {
                    foreach (KeyValuePair<IObject, IObject> pair in map)
                    {
                        if (pair.Key is null || pair.Value is null)
                        {
                            throw new ArgumentNullException("Invalid input key or value: Contains null object.");
                        }
                        if (pair.Key.BaseType > BaseType.MaxValue || pair.Value.BaseType > BaseType.MaxValue)
                        {
                            throw new ArgumentException("Invalid input key or value: Unsupported JCE type.");
                        }
                    }
                    keys.AddRange(map.Keys);
                    values.AddRange(map.Values);
                }
            }

            public bool ContainsKey(IObject key) => keys.Contains(key);

            public void Add(IObject key, IObject value)
            {
                if (key is null || value is null)
                {
                    throw new ArgumentNullException("Invalid input key or value: object is null.");
                }
                if (key.BaseType > BaseType.MaxValue || value.BaseType > BaseType.MaxValue)
                {
                    throw new ArgumentException("Invalid input key or value: Unsupported JCE type.");
                }
                if (keys.IndexOf(key) >= 0)
                {
                    throw new ArgumentException("An element with the same key already exists.");
                }
                keys.Add(key);
                values.Add(value);
            }

            public bool Remove(IObject key)
            {
                int index = keys.IndexOf(key);
                if (index >= 0)
                {
                    keys.RemoveAt(index);
                    values.RemoveAt(index);
                    return true;
                }
                return false;
            }

            public bool TryGetValue(IObject key, out IObject value)
            {
                int index = keys.IndexOf(key);
                if (index >= 0)
                {
                    value = values[index];
                    return true;
                }
                value = default;
                return false;
            }

            public void Add(KeyValuePair<IObject, IObject> item) =>
                Add(item.Key, item.Value);

            public void Clear()
            {
                keys.Clear();
                values.Clear();
            }

            public bool Contains(KeyValuePair<IObject, IObject> item)
            {
                int index = keys.IndexOf(item.Key);
                return index >= 0 && Equals(values[index], item.Value);
            }

            public void CopyTo(KeyValuePair<IObject, IObject>[] array, int arrayIndex) =>
                KeyValuePairs.CopyTo(array, arrayIndex);

            public bool Remove(KeyValuePair<IObject, IObject> item)
            {
                int index = keys.IndexOf(item.Key);
                if (index >= 0 && values[index].Equals(item.Value))
                {
                    keys.RemoveAt(index);
                    values.RemoveAt(index);
                    return true;
                }
                return false;
            }

            public IEnumerator<KeyValuePair<IObject, IObject>> GetEnumerator() => KeyValuePairs.GetEnumerator();

            public override bool Equals(object obj) =>
                obj is Map other &&
                Enumerable.SequenceEqual(keys, other.keys) &&
                Enumerable.SequenceEqual(values, other.values);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private readonly List<IObject> keys = new List<IObject>();
            private readonly List<IObject> values = new List<IObject>();

            private List<KeyValuePair<IObject, IObject>> KeyValuePairs
            {
                get
                {
                    int count = Count;
                    List<KeyValuePair<IObject, IObject>> pairs = new List<KeyValuePair<IObject, IObject>>(count);
                    for (int i = 0; i < count; ++i)
                    {
                        pairs.Add(new KeyValuePair<IObject, IObject>(keys[i], values[i]));
                    }
                    return pairs;
                }
            }
        }
    }
}
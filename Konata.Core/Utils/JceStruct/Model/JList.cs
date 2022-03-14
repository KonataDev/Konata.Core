using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model;

internal sealed class JList : IList<IObject>, IIndexable
{
    public Type Type => Type.List;

    public BaseType BaseType => BaseType.List;

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public JNumber Number => throw new InvalidCastException();

    public JFloat Float => throw new InvalidCastException();

    public JDouble Double => throw new InvalidCastException();

    public JString String => throw new InvalidCastException();

    JList IObject.List => this;

    public JMap Map => throw new InvalidCastException();

    public JStruct Struct => throw new InvalidCastException();

    public JSimpleList SimpleList => throw new InvalidCastException();

    public JKeyValuePair KeyValuePair => throw new InvalidCastException();

    public IObject this[int index]
    {
        get => list[index];
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException("Invalid input item: Contains null object.");
            }

            if (value.BaseType > BaseType.MaxValue)
            {
                throw new ArgumentException("Invalid input item: Unsupported JCE type.");
            }

            if (value.Type == Type.Null)
            {
                return;
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

    public JList(int capacity) => list.Capacity = capacity;

    public JList(IEnumerable<IObject> collection = null)
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

        if (item.Type == Type.Null)
        {
            return;
        }

        list.Insert(index, item);
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

        if (item.Type == Type.Null)
        {
            return;
        }

        list.Add(item);
    }

    public void Clear() => list.Clear();

    public bool Contains(IObject item) => list.Contains(item);

    public void CopyTo(IObject[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    public bool Remove(IObject item) => list.Remove(item);

    public IEnumerator<IObject> GetEnumerator() => list.GetEnumerator();

    public override bool Equals(object obj) =>
        obj is JList other &&
        Enumerable.SequenceEqual(list, other.list);

    public override int GetHashCode() => base.GetHashCode();

    public static implicit operator List<IObject>(JList value) => new List<IObject>(value);

    public static implicit operator JList(List<IObject> value) => new JList(value);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    private readonly List<IObject> list = new List<IObject>();
}

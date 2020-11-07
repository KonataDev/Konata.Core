using System;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public sealed class Struct : SortedDictionary<byte, IObject>, IIndexable
        {
            public Type Type => Type.StructBegin;

            public BaseType BaseType => BaseType.Struct;

            public Number Number => throw new InvalidCastException();

            public Float Float => throw new InvalidCastException();

            public Double Double => throw new InvalidCastException();

            public String String => throw new InvalidCastException();

            public List List => throw new InvalidCastException();

            public Map Map => throw new InvalidCastException();

            Struct IObject.Struct => this;

            public SimpleList SimpleList => throw new InvalidCastException();

            public KeyValuePair KeyValuePair => throw new InvalidCastException();

            public IObject this[string path]
            {
                get
                {
                    int dot = path.IndexOf('.');
                    if (dot >= 0)
                    {
                        byte tag = byte.Parse(path.Substring(0, dot));
                        if (ContainsKey(tag))
                        {
                            if (this[tag] is IIndexable ind)
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
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        byte tag = byte.Parse(path);
                        if (ContainsKey(tag))
                        {
                            return this[tag];
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
            }

            public Struct() : base() { }

            public Struct(IDictionary<byte, IObject> data) : base(data) { }

            public SimpleList Serialize() => new Struct { { 0, this } }.SerializeInternal();

            public SimpleList SerializeInternal() => new SimpleList(Jce.Serialize(this));

            public override bool Equals(object obj) =>
                obj is Struct other &&
                Enumerable.SequenceEqual(this, other);

            public override int GetHashCode() => base.GetHashCode();

            public static explicit operator byte[](Struct data) => Jce.Serialize(data);

            public static explicit operator Struct(byte[] data) => Deserialize(data);
        }
    }
}
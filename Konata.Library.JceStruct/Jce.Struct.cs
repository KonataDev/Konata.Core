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

            public override bool Equals(object obj) =>
                obj is Struct other &&
                Enumerable.SequenceEqual(this, other);

            public static implicit operator byte[](Struct data) => Serialize(data);

            public static implicit operator Struct(byte[] data) => Deserialize(data);
        }
    }
}
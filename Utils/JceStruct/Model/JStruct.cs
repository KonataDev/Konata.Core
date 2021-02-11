using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konata.Utils.JceStruct.Model
{
    public sealed class JStruct : SortedDictionary<byte, IObject>, IIndexable
    {
        public Type Type => Type.StructBegin;

        public BaseType BaseType => BaseType.Struct;

        public JNumber Number => throw new InvalidCastException();

        public JFloat Float => throw new InvalidCastException();

        public JDouble Double => throw new InvalidCastException();

        public JString String => throw new InvalidCastException();

        public JList List => throw new InvalidCastException();

        public JMap Map => throw new InvalidCastException();

        JStruct IObject.Struct => this;

        public JSimpleList SimpleList => throw new InvalidCastException();

        public JKeyValuePair KeyValuePair => throw new InvalidCastException();

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

        public JStruct() : base() { }

        public JStruct(IDictionary<byte, IObject> data) : base(data) { }

        public JSimpleList Serialize() => new JStruct { { 0, this } }.SerializeInternal();

        public JSimpleList SerializeInternal() => new JSimpleList(Jce.Serialize(this));

        public override bool Equals(object obj) =>
            obj is JStruct other &&
            Enumerable.SequenceEqual(this, other);

        public override int GetHashCode() => base.GetHashCode();

        public static explicit operator byte[](JStruct data) => Jce.Serialize(data);

        public static explicit operator JStruct(byte[] data) => Jce.Deserialize(data);
    }
}

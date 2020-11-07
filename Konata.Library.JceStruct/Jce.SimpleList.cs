using System;
using System.Linq;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct SimpleList : IIndexable
        {
            public Type Type => Value is null ? Type.Null : Type.SimpleList;

            public BaseType BaseType => BaseType.ByteArray;

            public long Length => Value is null ? 0 : Value.LongLength;

            public byte[] Value { get; set; }

            public Number Number => throw new InvalidCastException();

            public Float Float => throw new InvalidCastException();

            public Double Double => throw new InvalidCastException();

            public String String => throw new InvalidCastException();

            public List List => throw new InvalidCastException();

            public Map Map => DeserializeMap();

            public Struct Struct => Deserialize();

            SimpleList IObject.SimpleList => this;

            public KeyValuePair KeyValuePair => throw new InvalidCastException();

            public IObject this[string path]
            {
                get
                {
                    if (Value is null)
                    {
                        throw new NullReferenceException("Jce.Struct.Value is null.");
                    }
                    if (valueStruct is null || !Enumerable.SequenceEqual(Value, valueContent))
                    {
                        valueContent = new byte[Value.Length];
                        Value.CopyTo(valueContent, 0);
                        try
                        {
                            valueStruct = Jce.Deserialize(valueContent);
                        }
                        catch
                        {
                            throw new InvalidCastException();
                        }
                    }
                    return valueStruct[path];
                }
            }

            public SimpleList(byte[] value)
            {
                Value = value;
                valueStruct = null;
                valueContent = null;
            }

            public Struct Deserialize() => Jce.Deserialize(Value);

            public Map DeserializeMap() => Deserialize().First().Value.Map;

            public override bool Equals(object obj) =>
                obj is SimpleList other &&
                Enumerable.SequenceEqual(Value, other.Value);

            public override int GetHashCode() => base.GetHashCode();

            public static explicit operator byte[](SimpleList value) => value.Value;

            public static explicit operator SimpleList(byte[] value) => new SimpleList(value);

            private Struct valueStruct;
            private byte[] valueContent;
        }
    }
}
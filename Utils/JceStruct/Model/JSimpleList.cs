using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model
{
    public struct JSimpleList : IIndexable
    {
        public Type Type => Value is null ? Type.Null : Type.SimpleList;

        public BaseType BaseType => BaseType.ByteArray;

        public long Length => Value is null ? 0 : Value.LongLength;

        public byte[] Value { get; set; }

        public JNumber Number => throw new InvalidCastException();

        public JFloat Float => throw new InvalidCastException();

        public JDouble Double => throw new InvalidCastException();

        public JString String => throw new InvalidCastException();

        public JList List => throw new InvalidCastException();

        public JMap Map => DeserializeMap();

        public JStruct Struct => Deserialize();

        JSimpleList IObject.SimpleList => this;

        public JKeyValuePair KeyValuePair => throw new InvalidCastException();

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

        public JSimpleList(byte[] value)
        {
            Value = value;
            valueStruct = null;
            valueContent = null;
        }

        public JStruct Deserialize() => Jce.Deserialize(Value);

        public JMap DeserializeMap() => Deserialize().First().Value.Map;

        public override bool Equals(object obj) =>
            obj is JSimpleList other &&
            Enumerable.SequenceEqual(Value, other.Value);

        public override int GetHashCode() => base.GetHashCode();

        public static explicit operator byte[](JSimpleList value) => value.Value;

        public static explicit operator JSimpleList(byte[] value) => new JSimpleList(value);

        private JStruct valueStruct;
        private byte[] valueContent;
    }
}

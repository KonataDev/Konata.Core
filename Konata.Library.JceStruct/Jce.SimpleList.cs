using System;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct SimpleList : IIndexable
        {
            public Type Type
            {
                get
                {
                    return Type.SimpleList;
                }
            }

            public long Length
            {
                get
                {
                    return Value == null ? 0 : Value.LongLength;
                }
            }

            public byte[] Value { get; set; }

            public IObject this[string path]
            {
                get
                {
                    if (valueStruct == null)
                    {
                        try
                        {
                            valueStruct = Value;
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
            }

            public static implicit operator byte[](SimpleList value)
            {
                return value.Value;
            }

            public static implicit operator SimpleList(byte[] value)
            {
                return new SimpleList(value);
            }

            public static implicit operator Struct(SimpleList value)
            {
                return value.Value;
            }

            public static implicit operator SimpleList(Struct value)
            {
                return new SimpleList(value);
            }

            private Struct valueStruct;
        }
    }
}
using System;

#pragma warning disable CS0659 

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Number : IObject
        {
            public Type Type =>
                Value == 0 ? Type.ZeroTag :
                Value >= sbyte.MinValue && Value <= sbyte.MaxValue ? Type.Byte :
                Value >= short.MinValue && Value <= short.MaxValue ? Type.Short :
                Value >= int.MinValue && Value <= int.MaxValue ? Type.Int :
                Type.Long;

            public BaseType BaseType => BaseType.Number;

            public long Value { get; set; }

            public int ValueInt
            {
                get
                {
                    switch (Type)
                    {
                        case Type.ZeroTag:
                        case Type.Byte:
                        case Type.Short:
                        case Type.Int:
                            return (int)Value;
                        default:
                            throw new InvalidCastException();
                    }
                }
                set => Value = value;
            }

            public short ValueShort
            {
                get
                {
                    switch (Type)
                    {
                        case Type.ZeroTag:
                        case Type.Byte:
                        case Type.Short:
                            return (short)Value;
                        default:
                            throw new InvalidCastException();
                    }
                }
                set => Value = value;
            }

            public sbyte ValueByte
            {
                get
                {
                    switch (Type)
                    {
                        case Type.ZeroTag:
                        case Type.Byte:
                            return (sbyte)Value;
                        default:
                            throw new InvalidCastException();
                    }
                }
                set => Value = value;
            }

            public Number(long value) => Value = value;

            public override string ToString() => Value.ToString();

            public override bool Equals(object obj) =>
                obj is Number other &&
                Value.Equals(other.Value);

            public static implicit operator long(Number value) => value.Value;

            public static explicit operator Number(long value) => new Number(value);

            public static explicit operator Number(int value) => new Number(value);

            public static explicit operator Number(short value) => new Number(value);

            public static explicit operator Number(sbyte value) => new Number(value);
        }
    }
}

#pragma warning restore CS0659 

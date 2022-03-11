using System;

namespace Konata.Core.Utils.JceStruct.Model
{
    public struct JNumber : IObject
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
                        return (int) Value;
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
                        return (short) Value;
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
                        return (sbyte) Value;
                    default:
                        throw new InvalidCastException();
                }
            }
            set => Value = value;
        }

        JNumber IObject.Number => this;

        public JFloat Float => throw new InvalidCastException();

        public JDouble Double => throw new InvalidCastException();

        public JString String => throw new InvalidCastException();

        public JList List => throw new InvalidCastException();

        public JMap Map => throw new InvalidCastException();

        public JStruct Struct => throw new InvalidCastException();

        public JSimpleList SimpleList => throw new InvalidCastException();

        public JKeyValuePair KeyValuePair => throw new InvalidCastException();

        public JNumber(long value) => Value = value;

        public override string ToString() => Value.ToString();

        public override bool Equals(object obj) =>
            obj is JNumber other &&
            Value.Equals(other.Value);

        public override int GetHashCode() => base.GetHashCode();

        public static implicit operator long(JNumber value) => value.Value;

        public static explicit operator JNumber(long value) => new JNumber(value);

        public static explicit operator JNumber(int value) => new JNumber(value);

        public static explicit operator JNumber(short value) => new JNumber(value);

        public static explicit operator JNumber(sbyte value) => new JNumber(value);
    }
}

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Int8 : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Byte;
                }
            }

            public sbyte Value { get; set; }

            public Int8(sbyte value)
            {
                Value = value;
            }

            public static implicit operator sbyte(Int8 value)
            {
                return value.Value;
            }

            public static implicit operator Int8(sbyte value)
            {
                return new Int8(value);
            }
        }
    }
}
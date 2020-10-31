namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Int64 : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Long;
                }
            }

            public long Value { get; set; }

            public Int64(long value)
            {
                Value = value;
            }

            public static implicit operator long(Int64 value)
            {
                return value.Value;
            }

            public static implicit operator Int64(long value)
            {
                return new Int64(value);
            }
        }
    }
}
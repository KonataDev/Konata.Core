namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Int16 : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Short;
                }
            }

            public short Value { get; set; }

            public Int16(short value)
            {
                Value = value;
            }

            public static implicit operator short(Int16 value)
            {
                return value.Value;
            }

            public static implicit operator Int16(short value)
            {
                return new Int16(value);
            }
        }
    }
}
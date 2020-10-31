namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Int32 : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Int;
                }
            }

            public int Value { get; set; }

            public Int32(int value)
            {
                Value = value;
            }

            public static implicit operator int(Int32 value)
            {
                return value.Value;
            }

            public static implicit operator Int32(int value)
            {
                return new Int32(value);
            }
        }
    }
}
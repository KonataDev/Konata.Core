namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Float : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Float;
                }
            }

            public float Value { get; set; }

            public Float(float value)
            {
                Value = value;
            }

            public static implicit operator float(Float value)
            {
                return value.Value;
            }

            public static implicit operator Float(float value)
            {
                return new Float(value);
            }
        }
    }
}
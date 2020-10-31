namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Double : IObject
        {
            public Type Type
            {
                get
                {
                    return Value == 0 ? Type.ZeroTag : Type.Double;
                }
            }

            public double Value { get; set; }

            public Double(double value)
            {
                Value = value;
            }

            public static implicit operator double(Double value)
            {
                return value.Value;
            }

            public static implicit operator Double(double value)
            {
                return new Double(value);
            }
        }
    }
}
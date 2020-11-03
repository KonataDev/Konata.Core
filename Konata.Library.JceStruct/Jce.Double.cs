namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Double : IObject
        {
            public Type Type => Value == 0 ? Type.ZeroTag : Type.Double;

            public BaseType BaseType => BaseType.Double;

            public double Value { get; set; }

            public Double(double value) => Value = value;

            public override bool Equals(object obj) => obj is Double other && Value.Equals(other.Value);

            public static implicit operator double(Double value) => value.Value;

            public static implicit operator Double(double value) => new Double(value);
        }
    }
}
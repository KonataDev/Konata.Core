using System;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Double : IObject
        {
            public Type Type => Value == 0 ? Type.ZeroTag : Type.Double;

            public BaseType BaseType => BaseType.Double;

            public double Value { get; set; }

            public Number Number => throw new InvalidCastException();

            public Float Float => throw new InvalidCastException();

            Double IObject.Double => this;

            public String String => throw new InvalidCastException();

            public List List => throw new InvalidCastException();

            public Map Map => throw new InvalidCastException();

            public Struct Struct => throw new InvalidCastException();

            public SimpleList SimpleList => throw new InvalidCastException();

            public KeyValuePair KeyValuePair => throw new InvalidCastException();

            public Double(double value) => Value = value;

            public override string ToString() => Value.ToString();

            public override bool Equals(object obj) =>
                obj is Double other &&
                Value.Equals(other.Value);

            public override int GetHashCode() => base.GetHashCode();

            public static implicit operator double(Double value) => value.Value;

            public static implicit operator Double(double value) => new Double(value);
        }
    }
}
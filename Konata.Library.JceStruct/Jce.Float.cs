using System;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Float : IObject
        {
            public Type Type => Value == 0 ? Type.ZeroTag : Type.Float;

            public BaseType BaseType => BaseType.Float;

            public float Value { get; set; }

            public Number Number => throw new InvalidCastException();

            Float IObject.Float => this;

            public Double Double => throw new InvalidCastException();

            public String String => throw new InvalidCastException();

            public List List => throw new InvalidCastException();

            public Map Map => throw new InvalidCastException();

            public Struct Struct => throw new InvalidCastException();

            public SimpleList SimpleList => throw new InvalidCastException();

            public KeyValuePair KeyValuePair => throw new InvalidCastException();

            public Float(float value) => Value = value;

            public override string ToString() => Value.ToString();

            public override bool Equals(object obj) =>
                obj is Float other &&
                Value.Equals(other.Value);

            public override int GetHashCode() => base.GetHashCode();

            public static implicit operator float(Float value) => value.Value;

            public static implicit operator Float(float value) => new Float(value);
        }
    }
}
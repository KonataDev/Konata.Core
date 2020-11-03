namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct Float : IObject
        {
            public Type Type => Value == 0 ? Type.ZeroTag : Type.Float;

            public BaseType BaseType => BaseType.Float;

            public float Value { get; set; }

            public Float(float value) => Value = value;

            public override bool Equals(object obj) => obj is Float other && Value.Equals(other.Value);

            public static implicit operator float(Float value) => value.Value;

            public static implicit operator Float(float value) => new Float(value);
        }
    }
}
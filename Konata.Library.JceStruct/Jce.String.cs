using System;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct String : IObject
        {
            public Type Type => Value is null ? Type.Null : Value.Length <= byte.MaxValue ? Type.String1 : Type.String4;

            public BaseType BaseType => BaseType.String;

            public string Value { get; set; }

            public Number Number => throw new InvalidCastException();

            public Float Float => throw new InvalidCastException();

            public Double Double => throw new InvalidCastException();

            String IObject.String => this;

            public List List => throw new InvalidCastException();

            public Map Map => throw new InvalidCastException();

            public Struct Struct => throw new InvalidCastException();

            public SimpleList SimpleList => throw new InvalidCastException();

            public KeyValuePair KeyValuePair => throw new InvalidCastException();

            public String(string value) => Value = value;

            public override string ToString() => Value;

            public override bool Equals(object obj) =>
                obj is String other &&
                Value == other.Value;

            public override int GetHashCode() => base.GetHashCode();

            public static explicit operator string(String value) => value.Value;

            public static explicit operator String(string value) => new String(value);
        }
    }
}
using System;

#pragma warning disable CS0659 

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct String : IObject
        {
            public Type Type => Value is null ? Type.Null : Value.Length <= byte.MaxValue ? Type.String1 : Type.String4;

            public BaseType BaseType => BaseType.String;

            public string Value { get; set; }

            public String(string value) => Value = value;

            public override string ToString() => Value;

            public override bool Equals(object obj) =>
                obj is String other &&
                Value == other.Value;

            public static explicit operator string(String value) => value.Value;

            public static explicit operator String(string value) => new String(value);
        }
    }
}

#pragma warning restore CS0659 

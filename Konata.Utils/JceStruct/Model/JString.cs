using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Utils.JceStruct.Model
{
    public struct JString : IObject
    {
        public Type Type => Value is null ? Type.Null : Value.Length <= byte.MaxValue ? Type.String1 : Type.String4;

        public BaseType BaseType => BaseType.String;

        public string Value { get; set; }

        public JNumber Number => throw new InvalidCastException();

        public JFloat Float => throw new InvalidCastException();

        public JDouble Double => throw new InvalidCastException();

        JString IObject.String => this;

        public JList List => throw new InvalidCastException();

        public JMap Map => throw new InvalidCastException();

        public JStruct Struct => throw new InvalidCastException();

        public JSimpleList SimpleList => throw new InvalidCastException();

        public JKeyValuePair KeyValuePair => throw new InvalidCastException();

        public JString(string value) => Value = value;

        public override string ToString() => Value;

        public override bool Equals(object obj) =>
            obj is JString other &&
            Value == other.Value;

        public override int GetHashCode() => base.GetHashCode();

        public static explicit operator string(JString value) => value.Value;

        public static explicit operator JString(string value) => new JString(value);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model;

internal struct JFloat : IObject
{
    public Type Type => Value == 0 ? Type.ZeroTag : Type.Float;

    public BaseType BaseType => BaseType.Float;

    public float Value { get; set; }

    public JNumber Number => throw new InvalidCastException();

    JFloat IObject.Float => this;

    public JDouble Double => throw new InvalidCastException();

    public JString String => throw new InvalidCastException();

    public JList List => throw new InvalidCastException();

    public JMap Map => throw new InvalidCastException();

    public JStruct Struct => throw new InvalidCastException();

    public JSimpleList SimpleList => throw new InvalidCastException();

    public JKeyValuePair KeyValuePair => throw new InvalidCastException();

    public JFloat(float value) => Value = value;

    public override string ToString() => Value.ToString();

    public override bool Equals(object obj) =>
        obj is JFloat other &&
        Value.Equals(other.Value);

    public override int GetHashCode() => base.GetHashCode();

    public static implicit operator float(JFloat value) => value.Value;

    public static implicit operator JFloat(float value) => new JFloat(value);
}

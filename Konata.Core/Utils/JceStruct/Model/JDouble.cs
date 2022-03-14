using System;

namespace Konata.Core.Utils.JceStruct.Model;

internal struct JDouble : IObject
{
    public Type Type => Value == 0 ? Type.ZeroTag : Type.Double;

    public BaseType BaseType => BaseType.Double;

    public double Value { get; set; }

    public JNumber Number => throw new InvalidCastException();

    public JFloat Float => throw new InvalidCastException();

    JDouble IObject.Double => this;

    public JString String => throw new InvalidCastException();

    public JList List => throw new InvalidCastException();

    public JMap Map => throw new InvalidCastException();

    public JStruct Struct => throw new InvalidCastException();

    public JSimpleList SimpleList => throw new InvalidCastException();

    public JKeyValuePair KeyValuePair => throw new InvalidCastException();

    public JDouble(double value) => Value = value;

    public override string ToString() => Value.ToString();

    public override bool Equals(object obj) =>
        obj is JDouble other &&
        Value.Equals(other.Value);

    public override int GetHashCode() => base.GetHashCode();

    public static implicit operator double(JDouble value) => value.Value;

    public static implicit operator JDouble(double value) => new JDouble(value);
}

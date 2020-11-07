using System;
using System.Collections.Generic;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct KeyValuePair : IObject
        {
            public Type Type => Type.ZeroTag;

            public BaseType BaseType => BaseType.None;

            public IObject Key { get; }

            public IObject Value { get; }

            public Number Number => throw new InvalidCastException();

            public Float Float => throw new InvalidCastException();

            public Double Double => throw new InvalidCastException();

            public String String => throw new InvalidCastException();

            public List List => throw new InvalidCastException();

            public Map Map => throw new InvalidCastException();

            public Struct Struct => throw new InvalidCastException();

            public SimpleList SimpleList => throw new InvalidCastException();

            KeyValuePair IObject.KeyValuePair => this;

            public KeyValuePair(IObject key, IObject value)
            {
                Key = key;
                Value = value;
            }

            public override bool Equals(object obj) =>
                obj is KeyValuePair other &&
                Key.Equals(other.Key) &&
                Value.Equals(other.Value);

            public override int GetHashCode() => base.GetHashCode();

            public static implicit operator KeyValuePair<IObject, IObject>(KeyValuePair value) =>
                new KeyValuePair<IObject, IObject>(value.Key, value.Value);

            public static implicit operator KeyValuePair(KeyValuePair<IObject, IObject> value) =>
                new KeyValuePair(value.Key, value.Value);
        }
    }
}
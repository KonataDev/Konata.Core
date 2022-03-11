using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model
{
    public struct JKeyValuePair : IObject
    {
        public Type Type => Type.ZeroTag;

        public BaseType BaseType => BaseType.None;

        public IObject Key { get; }

        public IObject Value { get; }

        public JNumber Number => throw new InvalidCastException();

        public JFloat Float => throw new InvalidCastException();

        public JDouble Double => throw new InvalidCastException();

        public JString String => throw new InvalidCastException();

        public JList List => throw new InvalidCastException();

        public JMap Map => throw new InvalidCastException();

        public JStruct Struct => throw new InvalidCastException();

        public JSimpleList SimpleList => throw new InvalidCastException();

        JKeyValuePair IObject.KeyValuePair => this;

        public JKeyValuePair(IObject key, IObject value)
        {
            Key = key;
            Value = value;
        }

        public override bool Equals(object obj) =>
            obj is JKeyValuePair other &&
            Key.Equals(other.Key) &&
            Value.Equals(other.Value);

        public override int GetHashCode() => base.GetHashCode();

        public static implicit operator KeyValuePair<IObject, IObject>(JKeyValuePair value) =>
            new KeyValuePair<IObject, IObject>(value.Key, value.Value);

        public static implicit operator JKeyValuePair(KeyValuePair<IObject, IObject> value) =>
            new JKeyValuePair(value.Key, value.Value);
    }
}

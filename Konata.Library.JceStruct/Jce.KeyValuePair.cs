using System;
using System.Collections.Generic;

#pragma warning disable CS0659 

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

            public KeyValuePair(IObject key, IObject value)
            {
                Key = key;
                Value = value;
            }

            public override bool Equals(object obj) =>
                obj is KeyValuePair other &&
                Key.Equals(other.Key) &&
                Value.Equals(other.Value);

            public static implicit operator KeyValuePair<IObject, IObject>(KeyValuePair value) =>
                new KeyValuePair<IObject, IObject>(value.Key, value.Value);

            public static implicit operator KeyValuePair(KeyValuePair<IObject, IObject> value) =>
                new KeyValuePair(value.Key, value.Value);
        }
    }
}

#pragma warning restore CS0659 

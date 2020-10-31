namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public struct KeyValuePair : IObject
        {
            public Type Type
            {
                get
                {
                    return Type.None;
                }
            }

            public IObject Key { get; }

            public IObject Value { get; }

            public KeyValuePair(IObject key, IObject value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
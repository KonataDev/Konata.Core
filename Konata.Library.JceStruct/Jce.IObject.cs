namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public interface IObject
        {
            Type Type { get; }

            BaseType BaseType { get; }

            Number Number { get; }

            Float Float { get; }

            Double Double { get; }

            String String { get; }

            List List { get; }

            Map Map { get; }

            Struct Struct { get; }

            SimpleList SimpleList { get; }

            KeyValuePair KeyValuePair { get; }
        }
    }
}
namespace Konata.Core.Utils.JceStruct.Model;

internal interface IObject
{
    Type Type { get; }

    BaseType BaseType { get; }

    JNumber Number { get; }

    JFloat Float { get; }

    JDouble Double { get; }

    JString String { get; }

    JList List { get; }

    JMap Map { get; }

    JStruct Struct { get; }

    JSimpleList SimpleList { get; }

    JKeyValuePair KeyValuePair { get; }
}

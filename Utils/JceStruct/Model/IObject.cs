using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Utils.JceStruct.Model
{
    public interface IObject
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
}

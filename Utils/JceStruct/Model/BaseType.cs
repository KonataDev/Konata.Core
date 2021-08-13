using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model
{
    public enum BaseType : byte
    {
        Number,
        Float,
        Double,
        String,
        Map,
        List,
        Struct,
        ByteArray,
        MaxValue = ByteArray,
        None = byte.MaxValue
    }
}

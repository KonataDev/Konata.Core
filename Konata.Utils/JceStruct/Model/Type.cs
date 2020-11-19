using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Utils.JceStruct.Model
{
    public enum Type : byte
    {
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        String1,
        String4,
        Map,
        List,
        StructBegin,
        StructEnd,
        ZeroTag,
        SimpleList,
        Null = byte.MaxValue
    }
}

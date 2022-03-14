namespace Konata.Core.Utils.JceStruct.Model;

internal enum Type : byte
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

namespace Konata.Core.Utils.JceStruct.Model;

internal enum BaseType : byte
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

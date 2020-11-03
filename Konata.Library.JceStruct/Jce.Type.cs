namespace Konata.Library.JceStruct
{
    public static partial class Jce
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
            Unknown = byte.MaxValue
        }
    }
}
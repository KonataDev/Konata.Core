namespace Konata.Library.JceStruct
{
    public static partial class Jce
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
}
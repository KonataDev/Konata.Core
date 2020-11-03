namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public interface IObject
        {
            Type Type { get; }

            BaseType BaseType { get; }
        }
    }
}
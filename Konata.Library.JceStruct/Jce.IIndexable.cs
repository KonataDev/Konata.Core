namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public interface IIndexable : IObject
        {
            IObject this[string path] { get; }
        }
    }
}
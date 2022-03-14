namespace Konata.Core.Utils.JceStruct.Model;

internal interface IIndexable : IObject
{
    IObject this[string path] { get; }
}

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model;

public class JsonChain : BaseChain
{
    /// <summary>
    /// Json content
    /// </summary>
    public string Content { get; }

    private JsonChain(string json)
        : base(ChainType.Json, ChainMode.Singleton)
    {
        Content = json;
    }

    /// <summary>
    /// Create a json chain
    /// </summary>
    /// <param name="json"></param>
    public static JsonChain Create(string json)
        => new(json);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static JsonChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            return Create(UnEscape(args["content"]));
        }
    }

    public override string ToString()
        => $"[KQ:json,content={Escape(Content)}]";
    
    internal override string ToPreviewString()
        => "[JSON]";
}

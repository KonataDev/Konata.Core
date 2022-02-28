namespace Konata.Core.Message.Model;

public class TextChain : BaseChain
{
    public string Content { get; }

    private TextChain(string content)
        : base(ChainType.Text, ChainMode.Multiple)
    {
        Content = content;
    }

    /// <summary>
    /// Create a text chain
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TextChain Create(string text)
        => new(text);

    public override string ToString()
        => Content;
}

namespace Konata.Core.Message.Model;

public class TextChain : BaseChain
{
    public string Content { get; private set; }

    private TextChain(string content)
        : base(ChainType.Text, ChainMode.Multiple)
    {
        Content = content;
    }

    internal void Combine(TextChain chain)
        => Content += chain.Content;

    /// <summary>
    /// Create a text chain
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TextChain Create(string text)
        => new(text);

    public override string ToString()
        => Content;

    internal override string ToPreviewString()
        => Content.Length > 8 ? Content[..8] + "..." : Content;
}

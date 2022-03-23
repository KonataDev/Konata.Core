// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model;

public class XmlChain : BaseChain
{
    /// <summary>
    /// Xml content
    /// </summary>
    public string Content { get; internal set; }

    internal XmlChain(string xml)
        : base(ChainType.Xml, ChainMode.Singleton)
    {
        Content = xml;
    }

    /// <summary>
    /// Set xml content
    /// </summary>
    /// <param name="content"></param>
    internal void SetContent(string content)
        => Content = content;

    /// <summary>
    /// Create a xml chain
    /// </summary>
    /// <param name="xml"></param>
    public static XmlChain Create(string xml)
        => new(xml);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static XmlChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            return Create(UnEscape(args["content"]));
        }
    }

    public override string ToString()
        => $"[KQ:xml,content={Escape(Content)}]";

    internal override string ToPreviewString()
        => "[XML]";
}

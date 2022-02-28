// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Message.Model;

public class MultiMsgChain : XmlChain
{
    internal MessageChain Chain { get; }

    internal string Token { get; }

    private MultiMsgChain(MessageChain chain) : base("")
    {
        Chain = chain;
    }

    /// <summary>
    /// Create mulimsg chain
    /// </summary>
    /// <param name="chains"></param>
    public static MultiMsgChain Create(params BaseChain[] chains)
        => new(new MessageChain(chains));

    /// <summary>
    /// Create mulimsg chain
    /// </summary>
    /// <param name="chain"></param>
    public static MultiMsgChain Create(MessageChain chain)
        => new(chain);
}

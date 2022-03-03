using System.Collections.Generic;
using Konata.Core.Events.Model;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Message.Model;

public class MultiMsgChain : XmlChain
{
    /// <summary>
    /// Messages
    /// </summary>
    internal List<(SourceInfo info, MessageChain chain)> Messages { get; }

    /// <summary>
    /// Multimsg up information
    /// </summary>
    internal MultiMsgUpInfo MultiMsgUpInfo { get; private set; }

    /// <summary>
    /// Packed data
    /// </summary>
    internal byte[] PackedData { get; private set; }

    private MultiMsgChain() : base("")
    {
        Messages = new();
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(SourceInfo sourceInfo, MessageChain chain)
    {
        Messages.Add((sourceInfo, chain));
        return this;
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="chain"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(SourceInfo sourceInfo, BaseChain chain)
    {
        Messages.Add((sourceInfo, new MessageChain(chain)));
        return this;
    }

    /// <summary>
    /// Add message
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public MultiMsgChain AddMessage(SourceInfo sourceInfo, MessageBuilder builder)
    {
        Messages.Add((sourceInfo, builder.Build()));
        return this;
    }
    
    /// <summary>
    /// Set upload info
    /// </summary>
    /// <param name="info"></param>
    /// <param name="packed"></param>
    internal void SetMultiMsgUpInfo(MultiMsgUpInfo info, byte[] packed)
    {
        MultiMsgUpInfo = info;
        PackedData = packed;
    }

    /// <summary>
    /// Create mulimsg chain
    /// </summary>
    public static MultiMsgChain Create()
        => new();
    
    internal override string ToPreviewString()
        => "[聊天记录]";
}

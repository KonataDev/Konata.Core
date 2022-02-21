namespace Konata.Core.Events.Model;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class FriendMessageRecallEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Member uin <br/>
    /// </summary>
    public uint FriendUin { get; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Recall suffix <br/>
    /// </summary>
    public string RecallSuffix { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Message id <br/>
    /// </summary>
    public uint MessageId { get; }

    private FriendMessageRecallEvent(uint friendUin,
        uint messageId) : base(2000, true)
    {
        FriendUin = friendUin;
        MessageId = messageId;
    }

    private FriendMessageRecallEvent(int resultCode)
        : base(resultCode)
    {
    }

    private FriendMessageRecallEvent
        (uint friendUin, string recallSuffix) : base(0)
    {
        FriendUin = friendUin;
        RecallSuffix = recallSuffix;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Create
        (uint friendUin, uint messageId) => new(friendUin, messageId);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="recallSuffix"></param>
    /// <returns></returns>
    internal static FriendMessageRecallEvent Push(uint friendUin, string recallSuffix)
        => new(friendUin, recallSuffix);
}

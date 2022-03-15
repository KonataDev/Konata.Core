using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Events.Model;

public class FriendMessageEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Friend uin <br/>
    /// </summary>
    public uint FriendUin { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Message chain <br/>
    /// </summary>
    public MessageChain Message { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message sequence <br/>
    /// </summary>
    public uint MessageSequence { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message rand <br/>
    /// </summary>
    public uint MessageRand {get; private set;}

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message time <br/>
    /// </summary>
    public uint MessageTime { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message uuid <br/>
    /// </summary>
    public uint MessageUuid { get; private set; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Total slice count <br/>
    /// </summary>
    public uint SliceTotal { get; private set; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Current slice id <br/>
    /// </summary>
    public uint SliceIndex { get; private set; }

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Slice flags <br/>
    /// </summary>
    public uint SliceFlags { get; private set; }

    private FriendMessageEvent(uint friendUin, uint selfUin,
        MessageChain messageChain) : base(2000, true)
    {
        FriendUin = friendUin;
        SelfUin = selfUin;
        Message = messageChain;
    }

    private FriendMessageEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="messageChain"></param>
    /// <returns></returns>
    internal static FriendMessageEvent Create(uint friendUin, uint selfUin,
        MessageChain messageChain) => new(friendUin, selfUin, messageChain);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static FriendMessageEvent Result(int resultCode)
        => new(resultCode);

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="messageChain"></param>
    /// <returns></returns>
    internal static FriendMessageEvent Push(uint friendUin, uint selfUin,
        MessageChain messageChain)
        => new(friendUin, selfUin, messageChain);
}

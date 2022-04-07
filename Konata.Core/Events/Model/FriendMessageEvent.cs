using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Events.Model;

public class FriendMessageEvent : FilterableEvent
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
    public uint FriendUin => Message.Receiver.Uin == SelfUin
        ? Message.Sender.Uin
        : Message.Receiver.Uin;

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Message chain <br/>
    /// </summary>
    public MessageChain Chain
        => Message.Chain;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message <br/>
    /// </summary>
    public MessageStruct Message { get; private set; }

    private FriendMessageEvent(uint friendUin, uint selfUin,
        MessageChain messageChain) : base(true)
    {
        SelfUin = selfUin;
        Message = new MessageStruct(selfUin, "", friendUin, messageChain);
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
    /// Construct event result
    /// </summary>
    /// <returns></returns>
    internal static FriendMessageEvent Push()
        => new(0);

    /// <summary>
    /// Set message struct
    /// </summary>
    /// <param name="s"></param>
    internal void SetMessageStruct(MessageStruct s)
        => Message = s;

    /// <summary>
    /// Set self uin
    /// </summary>
    /// <param name="selfUin"></param>
    internal void SetSelfUin(uint selfUin)
        => SelfUin = selfUin;
}

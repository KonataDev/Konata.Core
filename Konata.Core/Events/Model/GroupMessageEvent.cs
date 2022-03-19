using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class GroupMessageEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Group uin
    /// </summary>
    public uint GroupUin
        => Message.Receiver.Uin;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Group name
    /// </summary>
    public string GroupName
        => Message.Receiver.Name;

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Member uin <br/>
    /// </summary>
    public uint MemberUin
        => Message.Sender.Uin;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Member card name <br/>
    /// </summary>
    public string MemberCard
        => Message.Sender.Name;

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

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Slice control <br/>
    /// </summary>
    internal MessageSlice SliceInfo { get; private set; }

    private GroupMessageEvent(uint groupUin, uint selfUin,
        MessageChain messageChain) : base(6000, true)
    {
        Message = new MessageStruct(selfUin, "", groupUin, messageChain);
    }

    private GroupMessageEvent(int resultCode)
        : base(resultCode)
    {
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="messageChain"></param>
    /// <returns></returns>
    internal static GroupMessageEvent Create(uint groupUin, uint selfUin,
        MessageChain messageChain) => new(groupUin, selfUin, messageChain);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <returns></returns>
    internal static GroupMessageEvent Result(int resultCode)
        => new(resultCode);
    
    /// <summary>
    /// Construct event push
    /// </summary>
    /// <returns></returns>
    internal static GroupMessageEvent Push()
        => new(0);
    
    /// <summary>
    /// Set message struct
    /// </summary>
    /// <param name="s"></param>
    internal void SetMessageStruct(MessageStruct s)
        => Message = s;

    /// <summary>
    /// Set slice info
    /// </summary>
    /// <param name="messageSlice"></param>
    internal void SetSliceInfo(MessageSlice messageSlice)
        => SliceInfo = messageSlice;
}

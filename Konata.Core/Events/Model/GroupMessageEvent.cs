using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class GroupMessageEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In] [Out]</b>     <br/>
    /// Group uin
    /// </summary>
    public uint GroupUin { get; private set; }

    /// <summary>
    /// <b>[Out]</b>          <br/>
    /// Group name
    /// </summary>
    public string GroupName { get; private set; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Member uin <br/>
    /// </summary>
    public uint MemberUin { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Member card name <br/>
    /// </summary>
    public string MemberCard { get; private set; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Message chain <br/>
    /// </summary>
    public MessageChain Message { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Source info <br/>
    /// </summary>
    public SourceInfo SourceInfo{ get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message sequence <br/>
    /// </summary>
    public uint MessageSequence
        => SourceInfo.MessageSeq;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message rand <br/>
    /// </summary>
    public uint MessageRand
        => SourceInfo.MessageRand;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message time <br/>
    /// </summary>
    public uint MessageTime
        => SourceInfo.MessageTime;

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Message uuid <br/>
    /// </summary>
    public uint MessageUuid
        => SourceInfo.MessageUuid;

    /// <summary>
    /// <b>[Opt] [Out]</b> <br/>
    /// Total slice count <br/>
    /// </summary>
    internal SliceControl SliceInfo { get; private set; }

    private GroupMessageEvent(uint groupUin, uint selfUin,
        MessageChain messageChain) : base(6000, true)
    {
        GroupUin = groupUin;
        MemberUin = selfUin;
        Message = messageChain;
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
    /// Set group name
    /// </summary>
    /// <param name="groupName"></param>
    internal void SetGroupName(string groupName)
        => GroupName = groupName;

    /// <summary>
    /// Set group uin
    /// </summary>
    /// <param name="groupUin"></param>
    internal void SetGroupUin(uint groupUin)
        => GroupUin = groupUin;

    /// <summary>
    /// Set message id
    /// </summary>
    /// <param name="sequence"></param>
    internal void SetMessageSequence(uint sequence)
        => MessageSequence = sequence;

    /// <summary>
    /// Set message rand
    /// </summary>
    /// <param name="rand"></param>
    internal void SetMessageRand(uint rand)
        => MessageRand = rand;

    /// <summary>
    /// Set message time
    /// </summary>
    /// <param name="time"></param>
    internal void SetMessageTime(uint time)
        => MessageTime = time;

    /// <summary>
    /// Set message uuid
    /// </summary>
    /// <param name="uuid"></param>
    internal void SetMessageUuid(uint uuid)
        => MessageUuid = uuid;

    /// <summary>
    /// Set message 
    /// </summary>
    /// <param name="message"></param>
    internal void SetMessage(MessageChain message)
        => Message = message;

    /// <summary>
    /// Set slice info
    /// </summary>
    /// <param name="slice"></param>
    internal void SetSliceInfo(SliceControl slice)
        => SliceInfo = slice;

    /// <summary>
    /// Set member uin
    /// </summary>
    /// <param name="memberUin"></param>
    internal void SetMemberUin(uint memberUin)
        => MemberUin = memberUin;

    /// <summary>
    /// Set member card
    /// </summary>
    /// <param name="memberCard"></param>
    internal void SetMemberCard(string memberCard)
        => MemberCard = memberCard;
}

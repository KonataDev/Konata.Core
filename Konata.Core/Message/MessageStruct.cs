using System;
using Konata.Core.Utils.Extensions;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

public class MessageInformation
{
    /// <summary>
    /// Time
    /// </summary>
    public uint Time { get; }

    /// <summary>
    /// Sequence
    /// </summary>
    public uint Sequence { get; }

    /// <summary>
    /// Uuid
    /// </summary>
    public uint Uuid { get; }

    /// <summary>
    /// Random
    /// </summary>
    public uint Random { get; }

    /// <summary>
    /// Source type
    /// </summary>
    public SourceType Type { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Message chain <br/>
    /// </summary>
    public MessageChain Chain { get; }

    /// <summary>
    /// <b>[In] [Out]</b>     <br/>
    /// Sender info
    /// </summary>
    public (uint Uin, string Name) Sender { get; private set; }

    /// <summary>
    /// <b>[In] [Out]</b>     <br/>
    /// Receiver info
    /// </summary>
    public (uint Uin, string Name) Receiver { get; private set; }

    public enum SourceType
    {
        Group,
        Friend
    }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="messageTime"></param>
    public MessageInformation(uint uin, string name, DateTime messageTime)
    {
        Sender = (uin, name);
        Time = (uint) (messageTime.ToUniversalTime().Epoch() / 1000);
    }

    /// <summary>
    /// Construct message information
    /// </summary>
    /// <param name="senderUin"></param>
    /// <param name="senderName"></param>
    /// <param name="receiverUin"></param>
    /// <param name="chain"></param>
    internal MessageInformation(uint senderUin, string senderName,
        uint receiverUin, MessageChain chain)
    {
        Chain = chain;
        Sender = (senderUin, senderName);
    }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    public MessageInformation(uint uin, string name)
        : this(uin, name, DateTime.Now)
    {
    }

    private MessageInformation(uint uin, uint squence, uint rand, uint time, uint uuid)
    {
        Uin = uin;
        Sequence = squence;
        Random = rand;
        Time = time;
        Uuid = uuid;
    }

    /// <summary>
    /// Set member info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    internal void SetSenderInfo(uint uin, string name)
        => Sender = (uin, name);

    /// <summary>
    /// Set group info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// 
    internal void SetReceiverInfo(uint uin, string name)
        => Receiver = (uin, name);
}

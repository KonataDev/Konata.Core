using System;
using Konata.Core.Utils.Extensions;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

public class MessageStruct
{
    /// <summary>
    /// Time
    /// </summary>
    public uint Time { get; private set; }

    /// <summary>
    /// Sequence
    /// </summary>
    public uint Sequence { get; private set; }

    /// <summary>
    /// Uuid
    /// </summary>
    public long Uuid { get; private set; }

    /// <summary>
    /// Random
    /// </summary>
    public uint Random { get; private set; }

    /// <summary>
    /// Source type
    /// </summary>
    public SourceType Type { get; private set; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Message chain <br/>
    /// </summary>
    public MessageChain Chain { get; private set; }

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
        Friend,
        Stranger
    }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="messageTime"></param>
    public MessageStruct(uint uin, string name, DateTime messageTime)
    {
        Sender = (uin, name);
        Time = (uint) (messageTime.ToUniversalTime().Epoch() / 1000);
    }

    /// <summary>
    /// Construct message information for message outgoing
    /// </summary>
    /// <param name="senderUin"></param>
    /// <param name="senderName"></param>
    /// <param name="receiverUin"></param>
    /// <param name="chain"></param>
    internal MessageStruct(uint senderUin, string senderName,
        uint receiverUin, MessageChain chain)
    {
        Chain = chain;
        Sender = (senderUin, senderName);
        Receiver = (receiverUin, "");
    }

    /// <summary>
    /// Construct message information for message incoming
    /// </summary>
    /// <param name="type"></param>
    internal MessageStruct(SourceType type) => Type = type;

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// <param name="chain"></param>
    /// <param name="type"></param>
    public MessageStruct(uint uin, string name, MessageChain chain,
        SourceType type = SourceType.Group) : this(uin, name, DateTime.Now)
    {
        Type = type;
        Chain = chain;

        var rand = new Random();
        Random = (uint)rand.Next();
        Uuid =  rand.Next();
    }

    /// <summary>
    /// Set source info
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="rand"></param>
    /// <param name="time"></param>
    /// <param name="uuid"></param>
    internal void SetSourceInfo(uint sequence, uint rand, uint time, long uuid)
    {
        Sequence = sequence;
        Random = rand;
        Time = time;
        Uuid = uuid;
    }

    /// <summary>
    /// Set sender info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    internal void SetSenderInfo(uint uin, string name)
        => Sender = (uin, name);

    /// <summary>
    /// Set sender uin
    /// </summary>
    /// <param name="uin"></param>
    internal void SetSenderUin(uint uin)
        => Sender = (uin, Sender.Name ?? "");

    /// <summary>
    /// Set sender name
    /// </summary>
    /// <param name="name"></param>
    internal void SetSenderName(string name)
        => Sender = (Sender.Uin, name);

    /// <summary>
    /// Set group info
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="name"></param>
    /// 
    internal void SetReceiverInfo(uint uin, string name)
        => Receiver = (uin, name);

    /// <summary>
    /// Set receiver uin
    /// </summary>
    /// <param name="uin"></param>
    internal void SetReceiverUin(uint uin)
        => Receiver = (uin, Receiver.Name ?? "");

    /// <summary>
    /// Set receiver name
    /// </summary>
    /// <param name="name"></param>
    internal void SetReceiverName(string name)
        => Receiver = (Receiver.Uin, name);

    /// <summary>
    /// Set message
    /// </summary>
    /// <param name="chain"></param>
    internal void SetMessage(MessageChain chain)
        => Chain = chain;
}

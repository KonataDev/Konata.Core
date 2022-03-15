using System;
using Konata.Core.Events.Model;
using Konata.Core.Utils.Extensions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

public class SourceInfo
{
    /// <summary>
    /// Source uin
    /// </summary>
    public uint SourceUin { get; }

    /// <summary>
    /// Source name
    /// </summary>
    public string SourceName { get; }

    /// <summary>
    /// Time
    /// </summary>
    public uint MessageTime { get; }

    /// <summary>
    /// Sequence
    /// </summary>
    public uint MessageSeq { get; }

    /// <summary>
    /// Uuid
    /// </summary>
    public uint MessageUuid { get; }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    /// <param name="messageTime"></param>
    public SourceInfo(uint sourceUin, string sourceName, DateTime messageTime)
    {
        SourceUin = sourceUin;
        SourceName = sourceName;
        MessageSeq = 0;
        MessageUuid = 0;
        MessageTime = (uint) (messageTime.ToUniversalTime().Epoch() / 1000);
    }

    /// <summary>
    /// Construct fake source info
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    public SourceInfo(uint sourceUin, string sourceName)
        : this(sourceUin, sourceName, DateTime.Now)
    {
    }

    /// <summary>
    /// Construct source info from group message
    /// </summary>
    private SourceInfo(GroupMessageEvent e)
    {
        SourceUin = e.MemberUin;
        SourceName = e.MemberCard;
        MessageTime = e.MessageTime;
        MessageUuid = e.MessageUuid;
        MessageSeq = e.MessageSequence;
    }

    /// <summary>
    /// Construct source info from group message
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(GroupMessageEvent e)
        => new(e);

    /// <summary>
    /// Construct source info from friend message
    /// TODO Friend name
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(FriendMessageEvent e)
        => new(e.FriendUin, "", e.EventTime);
}

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
    /// Uniseq
    /// </summary>
    public uint MessageUniSeq { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    /// <param name="messageTime"></param>
    public SourceInfo(uint sourceUin, string sourceName, uint messageTime)
    {
        MessageTime = messageTime;
        SourceUin = sourceUin;
        SourceName = sourceName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    /// <param name="messageTime"></param>
    public SourceInfo(uint sourceUin, string sourceName, DateTime messageTime)
        : this(sourceUin, sourceName, (uint) (messageTime.Epoch() / 1000))
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    public SourceInfo(uint sourceUin, string sourceName)
        : this(sourceUin, sourceName, DateTime.Now.ToUniversalTime())
    {
    }

    /// <summary>
    /// Create source info from group message
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(GroupMessageEvent e)
        => new(e.MemberUin, e.MemberCard, e.MessageTime);

    /// <summary>
    /// Create source info from friend message
    /// TODO Friend name
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(FriendMessageEvent e)
        => new(e.FriendUin, "", e.EventTime);
}

using System;
using Konata.Core.Events.Model;

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
    public DateTime Time { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    /// <param name="time"></param>
    public SourceInfo(uint sourceUin, string sourceName, DateTime time)
    {
        Time = time;
        SourceUin = sourceUin;
        SourceName = sourceName;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceUin"></param>
    /// <param name="sourceName"></param>
    public SourceInfo(uint sourceUin, string sourceName)
        : this(sourceUin, sourceName, DateTime.Now)
    {
    }

    /// <summary>
    /// Create source info from group message
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(GroupMessageEvent e)
        => new(e.MemberUin, e.MemberCard, e.EventTime);

    /// <summary>
    /// Create source info from friend message
    /// TODO Friend name
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static SourceInfo From(FriendMessageEvent e)
        => new(e.FriendUin, "", e.EventTime);
}

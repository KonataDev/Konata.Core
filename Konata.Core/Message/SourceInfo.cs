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
    /// Rand
    /// </summary>
    public uint MessageRand { get; }

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

    private SourceInfo(uint sourceUin, uint msgSequence, uint msgRand, uint msgTime)
    {
        SourceUin = sourceUin;
        MessageSeq = msgSequence;
        MessageRand = msgRand;
        MessageTime = msgTime;
    }

    internal static SourceInfo From(uint sourceUin, uint msgSequence, uint msgRand, uint msgTime)
        => new(sourceUin, msgSequence, msgRand, msgTime);
}

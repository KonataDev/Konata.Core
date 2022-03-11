// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace Konata.Core.Message.Model;

public class ReplyChain : BaseChain
{
    /// <summary>
    /// Reply uin
    /// </summary>
    public uint ReplyUin { get; }

    /// <summary>
    /// Message id
    /// </summary>
    public uint SourceId { get; }

    /// <summary>
    /// Reply time
    /// </summary>
    public uint ReplyTime { get; }

    /// <summary>
    /// Reply source
    /// </summary>
    public BaseChain Source { get; }

    /// <summary>
    /// Reply content
    /// </summary>
    public BaseChain Content { get; }

    private ReplyChain(uint sourceId, uint replyUin, uint replyTime)
        : base(ChainType.Reply, ChainMode.Singletag)
    {
        SourceId = sourceId;
        ReplyUin = replyUin;
        ReplyTime = replyTime;
    }

    /// <summary>
    /// Create a reply chain
    /// </summary>
    /// <param name="sourceId"></param>
    /// <param name="replyUin"></param>
    /// <param name="replyTime"></param>
    /// <returns></returns>
    internal static ReplyChain Create(uint sourceId, uint replyUin, uint replyTime)
    {
        return new(sourceId, replyUin, replyTime);
    }

    /// <summary>
    /// Create a reply chain
    /// </summary>
    /// <param name="source"></param>
    /// <param name="reply"></param>
    /// <returns></returns>
    internal static ReplyChain Create(BaseChain source, BaseChain reply)
    {
        // TODO
        return null;
    }

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static ReplyChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            var qq = uint.Parse(args["qq"]);
            var source = uint.Parse(args["id"]);
            var time = uint.Parse(args["time"]);

            return Create(source, qq, time);
        }
    }

    public override string ToString()
        => $"[KQ:reply," +
           $"qq={ReplyUin}," +
           $"id={SourceId}," +
           $"time={ReplyTime}]";

    internal override string ToPreviewString()
        => "[回复]";
}

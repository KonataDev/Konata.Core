// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnassignedGetOnlyAutoProperty

using Konata.Core.Utils.IO;

namespace Konata.Core.Message.Model;

public class ReplyChain : BaseChain
{
    /// <summary>
    /// Reply uin
    /// </summary>
    public uint Uin { get; }

    /// <summary>
    /// Sequence of reply message
    /// </summary>
    public uint Sequence { get; }

    /// <summary>
    /// Uuid of reply message
    /// </summary>
    public long Uuid { get; }

    /// <summary>
    /// Time of reply message
    /// </summary>
    public uint Time { get; }

    /// <summary>
    /// Reply message preview
    /// </summary>
    public string Preview { get; }

    private ReplyChain(uint uin, uint sequence, long uuid, uint time, string preview)
        : base(ChainType.Reply, ChainMode.Singletag)
    {
        Uin = uin;
        Sequence = sequence;
        Uuid = uuid;
        Time = time;
        Preview = preview;
    }

    /// <summary>
    /// Create a reply chain
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public static ReplyChain Create(MessageStruct reference)
        => new(reference.Sender.Uin, reference.Sequence, reference.Uuid, reference.Time, reference.Chain.ToPreviewString());

    /// <summary>
    /// Create a reply chain
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="sequence"></param>
    /// <param name="uuid"></param>
    /// <param name="time"></param>
    /// <param name="preview"></param>
    /// <returns></returns>
    internal static ReplyChain Create(uint uin, uint sequence, long uuid, uint time, string preview)
        => new(uin, sequence, uuid, time, preview);

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
            var seq = uint.Parse(args["seq"]);
            var uuid = long.Parse(args["uuid"]);
            var time = uint.Parse(args["time"]);
            var content = ByteConverter.UnBase64String(args["content"]);

            return Create(qq, seq, uuid, time, content);
        }
    }

    public override string ToString()
        => $"[KQ:reply," +
           $"qq={Uin}," +
           $"seq={Sequence}," +
           $"uuid={Uuid}," +
           $"time={Time}," +
           $"content={Preview}]";

    internal override string ToPreviewString()
        => "[回复]";
}

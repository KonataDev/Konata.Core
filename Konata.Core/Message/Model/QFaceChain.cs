// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model;

public class QFaceChain : BaseChain
{
    /// <summary>
    /// Face id
    /// </summary>
    public uint FaceId { get; }

    /// <summary>
    /// Is big face
    /// </summary>
    public bool Big { get; }

    private QFaceChain(uint face, bool big)
        : base(ChainType.QFace, ChainMode.Multiple)
    {
        Big = big;
        FaceId = face;

        // Convert to singleton mode
        // if this is a big qface
        if (big) Mode = ChainMode.Singleton;
    }

    /// <summary>
    /// Create a qface chain
    /// </summary>
    /// <param name="id"></param>
    /// <param name="big"></param>
    /// <returns></returns>
    internal static QFaceChain Create(uint id, bool big = false)
        => new(id, big);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static QFaceChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            args.TryGetValue("big", out var __);
            var id = uint.Parse(args["id"]);
            var isbig = __?.ToLower() == "true";

            return Create(id, isbig);
        }
    }

    public override string ToString()
        => $"[KQ:face,id={FaceId}{(Big ? ",big=true" : "")}]";

    internal override string ToPreviewString()
        => "[表情]";
}

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model;

public class QFaceChain : BaseChain
{
    public uint FaceId { get; }

    private QFaceChain(uint face)
        : base(ChainType.QFace, ChainMode.Multiple)
    {
        FaceId = face;
    }

    /// <summary>
    /// Create a qface chain
    /// </summary>
    /// <param name="face"></param>
    /// <returns></returns>
    internal static QFaceChain Create(uint face)
        => new(face);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static QFaceChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            return Create(uint.Parse(args["id"]));
        }
    }

    public override string ToString()
        => $"[KQ:face,id={FaceId}]";
}

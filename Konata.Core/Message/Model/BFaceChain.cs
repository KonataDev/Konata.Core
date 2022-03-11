using Konata.Core.Utils.IO;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Message.Model;

public class BFaceChain : BaseChain
{
    public string Name { get; }

    public uint FaceId { get; }

    public string FaceHash { get; }

    public byte[] HashData { get; }

    private BFaceChain(string name, uint faceid, string hashstr)
        : base(ChainType.BFace, ChainMode.Singleton)
    {
        Name = name;
        FaceId = faceid;
        HashData = ByteConverter.UnHex(hashstr);
        FaceHash = hashstr;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal BFaceChain Create(string name,
        uint faceid, string hashstr) => new(name, faceid, hashstr);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static BaseChain Parse(string code)
    {
        return null;
    }

    public override string ToString()
        => $"[KQ:bface,id={FaceId}]";

    internal override string ToPreviewString()
        => "[原创表情]";
}

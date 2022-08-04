namespace Konata.Core.Message;

public class FileChain: BaseChain
{
    public string FileName { get; set; }
    public ulong FileSize { get; set; }
    public string FileUrl { get; set; }
    public byte[] FileHash { get; set; }
    
    internal string FileUuid { get; set; }
    
    internal FileChain(string fileName, ulong fileSize, string fileUrl, byte[] fileHash, string fileUuid)
        : base(ChainType.File, ChainMode.Singleton)
    {
        FileName = fileName;
        FileSize = fileSize;
        FileUrl = fileUrl;
        FileHash = fileHash;
        FileUuid = fileUuid;
    }
    
    internal FileChain(string fileName, ulong fileSize, byte[] fileHash, string fileUuid)
        : base(ChainType.File, ChainMode.Singleton)
    {
        FileName = fileName;
        FileSize = fileSize;
        FileHash = fileHash;
        FileUuid = fileUuid;
    }

    internal override string ToPreviewString()
        => FileName.Length > 8 ? FileName[..8] + "..." : FileName;
}
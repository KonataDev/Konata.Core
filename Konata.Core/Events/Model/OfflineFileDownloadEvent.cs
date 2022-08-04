namespace Konata.Core.Events.Model;

public class OfflineFileDownloadEvent : ProtocolEvent
{
    /// <summary>
    /// The bot url
    /// </summary>
    public uint SelfUin { get; }
    
    /// <summary>
    /// The UUID defined in the fileChain or preview packet
    /// </summary>
    internal string FileUuid { get; }
    
    /// <summary>
    /// The download url
    /// </summary>
    public string FileUrl { get; }


    private OfflineFileDownloadEvent(uint selfUin, string fileUuid) : base(true)
    {
        SelfUin = selfUin;
        FileUuid = fileUuid;
    }

    private OfflineFileDownloadEvent(string url): base(0)
    {
        FileUrl = url;
    }

    internal static OfflineFileDownloadEvent Create(uint selfUin, string fileUuid) => 
        new(selfUin, fileUuid);
    
    internal static OfflineFileDownloadEvent Result(string fileUrl) => 
        new(fileUrl);
}
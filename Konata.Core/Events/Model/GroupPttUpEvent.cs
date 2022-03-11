using Konata.Core.Message.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events.Model;

public class GroupPttUpEvent : ProtocolEvent
{
    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Group uin <br/>
    /// </summary>
    public uint GroupUin { get; }

    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Record to upload <br/>
    /// </summary>
    public RecordChain UploadRecord { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Record upload info <br/>
    /// </summary>
    public PttUpInfo UploadInfo { get; }

    private GroupPttUpEvent(uint groupUin, uint selfUin,
        RecordChain uploadRecord) : base(6000, true)
    {
        GroupUin = groupUin;
        SelfUin = selfUin;
        UploadRecord = uploadRecord;
    }

    private GroupPttUpEvent(int resultCode,
        PttUpInfo uploadInfo) : base(resultCode)
    {
        UploadInfo = uploadInfo;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="uploadRecord"></param>
    /// <returns></returns>
    internal static GroupPttUpEvent Create(uint groupUin, uint selfUin,
        RecordChain uploadRecord) => new(groupUin, selfUin, uploadRecord);

    internal static GroupPttUpEvent Result(int resultCode,
        PttUpInfo uploadInfo) => new(resultCode, uploadInfo);
}

public class PttUpInfo
{
    public uint Ip { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public byte[] Ukey { get; set; }

    public string FileKey { get; set; }

    public uint UploadId { get; set; }

    public byte[] UploadTicket { get; set; }
}

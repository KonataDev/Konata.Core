using Konata.Core.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class MultiMsgApplyUpEvent : ProtocolEvent
{
    /// <summary>
    /// 
    /// </summary>
    public uint DestUin { get; }

    /// <summary>
    /// 
    /// </summary>
    public byte[] Md5Hash { get; }

    /// <summary>
    /// 
    /// </summary>
    public uint PackedLength { get; }

    public MultiMsgUpInfo UploadInfo { get; }

    private MultiMsgApplyUpEvent(uint destUin, uint packedLen, byte[] md5Hash)
        : base(6000, true)
    {
        DestUin = destUin;
        Md5Hash = md5Hash;
        PackedLength = packedLen;
    }

    private MultiMsgApplyUpEvent(int resultCode,
        MultiMsgUpInfo uploadInfo) : base(resultCode)
    {
        UploadInfo = uploadInfo;
    }

    /// <summary>
    /// Construct event
    /// </summary>
    /// <param name="destUin"></param>
    /// <param name="packed"></param>
    /// <returns></returns>
    public static MultiMsgApplyUpEvent Create(uint destUin, byte[] packed)
        => new(destUin, (uint) packed.Length, packed.Md5());

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="uploadInfo"></param>
    /// <returns></returns>
    internal static MultiMsgApplyUpEvent Result(int resultCode,
        MultiMsgUpInfo uploadInfo) => new(resultCode, uploadInfo);
}

internal class MultiMsgUpInfo
{
    public uint Ip { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public byte[] UploadTicket { get; set; }

    public string MsgResId { get; set; }

    public byte[] MsgUKey { get; set; }
}

public enum MultiMsgType
{
    Group,
    Friend
}

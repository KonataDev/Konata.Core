using System.Collections.Generic;
using Konata.Core.Message.Model;

namespace Konata.Core.Events.Model;

internal class PicUpEvent : ProtocolEvent
{
    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Destination uin <br/>
    /// [UpMode.GroupUp] Group uin <br/>
    /// [UpMode.OffUp] Friend uin <br/>
    /// </summary>
    public uint DestUin { get; }

    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>s
    /// <b>[In]</b> <br/>
    /// Image to upload <br/>
    /// </summary>
    public List<ImageChain> UploadImages { get; }

    /// <summary>
    /// <b>[In] [Out]</b> <br/>
    /// Image upload info <br/>
    /// </summary>
    internal List<PicUpInfo> UploadInfo { get; }

    /// <summary>
    ///  <b>[In]</b> <br/>
    /// Image upload mode
    /// </summary>
    internal UpMode Mode { get; }

    private PicUpEvent(uint destUin, uint selfUin, UpMode mode,
        List<ImageChain> uploadImages) : base(true)
    {
        DestUin = destUin;
        SelfUin = selfUin;
        UploadImages = uploadImages;
        Mode = mode;
    }

    private PicUpEvent(int resultCode,
        List<PicUpInfo> uploadInfo) : base(resultCode)
    {
        UploadInfo = uploadInfo;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="uploadImages"></param>
    /// <returns></returns>
    internal static PicUpEvent GroupUp(uint groupUin, uint selfUin,
        List<ImageChain> uploadImages) => new(groupUin, selfUin, UpMode.GroupUp, uploadImages);

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="selfUin"></param>
    /// <param name="uploadImages"></param>
    /// <returns></returns>
    internal static PicUpEvent OffUp(uint friendUin, uint selfUin,
        List<ImageChain> uploadImages) => new(friendUin, selfUin, UpMode.OffUp, uploadImages);

    /// <summary>
    /// Construct event result
    /// </summary>
    /// <param name="resultCode"></param>
    /// <param name="uploadInfo"></param>
    /// <returns></returns>
    internal static PicUpEvent Result(int resultCode,
        List<PicUpInfo> uploadInfo) => new(resultCode, uploadInfo);
}

internal class PicUpInfo
{
    public uint Ip { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public object UploadId { get; set; }

    public byte[] UploadTicket { get; set; }

    public bool UseCached { get; set; }

    public CachedPicInfo CachedInfo { get; set; }
}

internal class CachedPicInfo
{
    public ImageType Type { get; set; }

    public byte[] Hash { get; set; }

    public uint Width { get; set; }

    public uint Height { get; set; }

    public uint Length { get; set; }
}

internal enum UpMode
{
    GroupUp,
    OffUp
}

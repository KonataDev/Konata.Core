using System.Collections.Generic;
using Konata.Core.Message.Model;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

public class LongConnOffPicUpEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Self uin <br/>
    /// </summary>
    public uint SelfUin { get; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image to upload <br/>
    /// </summary>
    public List<ImageChain> UploadImages { get; }

    private LongConnOffPicUpEvent(uint selfUin,
        List<ImageChain> uploadImages) : base(true)
    {
        SelfUin = selfUin;
        UploadImages = uploadImages;
    }

    /// <summary>
    /// Construct event request
    /// </summary>
    /// <param name="selfUin"></param>
    /// <param name="uploadImages"></param>
    /// <returns></returns>
    internal static LongConnOffPicUpEvent Create(uint selfUin,
        List<ImageChain> uploadImages) => new(selfUin, uploadImages);
}

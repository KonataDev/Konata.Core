using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model;

internal class ImageOcrEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image url (uploaded via highway)
    /// Example: https://qqpicocr-75402.gzc.vod.tencent-cloud.com/xxxxx
    /// </summary>
    public string ImageUrl { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image MD5
    /// </summary>
    public string ImageMd5 { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image width
    /// </summary>
    public uint ImageWidth { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image height
    /// </summary>
    public uint ImageHeight { get; private set; }

    /// <summary>
    /// <b>[In]</b> <br/>
    /// Image length
    /// </summary>
    public uint ImageLength { get; private set; }

    /// <summary>
    /// <b>[Out]</b> <br/>
    /// OCR result
    /// </summary>
    public List<ImageOcrResult> OcrResult { get; private set; }

    public ImageOcrEvent(string imageUrl, string imageMd5,
        uint imageWidth, uint imageHeight, uint imageLength) : base(true)
    {
        ImageUrl = imageUrl;
        ImageMd5 = imageMd5;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
        ImageLength = imageLength;
    }

    public ImageOcrEvent(int resultCode, List<ImageOcrResult> result) : base(resultCode)
    {
        OcrResult = result;
    }

    public static ImageOcrEvent Create(string imageUrl, string imageMd5,
        uint imageWidth, uint imageHeight, uint imageLength) => new(imageUrl, imageMd5, imageWidth, imageHeight, imageLength);

    public static ImageOcrEvent Result(int code, List<ImageOcrResult> result)
        => new(code, result);
}

public class ImageOcrResult
{
    // public int X { get; }
    //
    // public int Y { get; }

    public string Text { get; }

    public uint Confidence { get; }

    public ImageOcrResult(int x, int y, string text, uint confidence)
    {
        // X = x;
        // Y = y;
        Text = text;
        Confidence = confidence;
    }
}

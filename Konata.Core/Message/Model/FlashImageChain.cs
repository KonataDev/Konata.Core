using System.IO;

// ReSharper disable UnusedMember.Global

namespace Konata.Core.Message.Model;

public class FlashImageChain : ImageChain
{
    private FlashImageChain(ImageChain chain)
    {
        Type = ChainType.Flash;
        Mode = ChainMode.Singleton;

        ImageUrl = chain.ImageUrl;
        FileData = chain.FileData;
        FileLength = chain.FileLength;
        Width = chain.Width;
        Height = chain.Height;
        HashData = chain.HashData;
        FileHash = chain.FileHash;
        ImageType = chain.ImageType;
        FileName = chain.FileName;

        if (chain.PicUpInfo != null)
            SetPicUpInfo(chain.PicUpInfo);
    }

    /// <summary>
    /// Create a flash image chain
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public new static FlashImageChain CreateFromFile(string filepath)
        => new(ImageChain.CreateFromFile(filepath));

    /// <summary>
    /// Create a flash image chain from plain base64 <br />
    /// Not including the header 'base64://'
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public new static FlashImageChain CreateFromBase64(string base64)
        => new(ImageChain.CreateFromBase64(base64));

    /// <summary>
    /// Create a flash image chain from url (limit 10MB)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public new static FlashImageChain CreateFromUrl(string url)
        => new(ImageChain.CreateFromUrl(url));

    /// <summary>
    /// Create a flash image chain
    /// </summary>
    /// <param name="chain"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static FlashImageChain CreateFromImageChain(ImageChain chain)
        => new(chain);

    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal new static FlashImageChain Parse(string code)
        => CreateFromImageChain(ImageChain.Parse(code));

    public override string ToString()
        => $"[KQ:flash," +
           $"file={FileName}," +
           $"width={Width}," +
           $"height={Height}," +
           $"length={FileLength}," +
           $"type={(int) ImageType}]";

    internal override string ToPreviewString()
        => "[闪照]";
}

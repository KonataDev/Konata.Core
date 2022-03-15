using System;
using System.IO;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;
using Konata.Core.Events.Model;
using Konata.Core.Utils.FileFormat;

// ReSharper disable InvertIf
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model;

public class FlashImageChain : ImageChain
{
    private FlashImageChain(ImageChain chain)
        : base(chain.ImageUrl, chain.FileName, chain.FileHash, 
        chain.Width,chain.Height, chain.FileLength, chain.ImageType)
    {
        Type = ChainType.Flash;
        Mode = ChainMode.Singleton;
    }

    protected FlashImageChain(string url, string fileName,
        string fileHash, uint width, uint height, uint length, ImageType type)
        : base(url, fileName, fileHash, width, height, length, type)
    {
        Type = ChainType.Flash;
        Mode = ChainMode.Singleton;
    }

    protected FlashImageChain(byte[] data, uint width,
        uint height, byte[] md5, string md5str, ImageType type)
        : base(data, width, height, md5, md5str, type)
    {
        Type = ChainType.Flash;
        Mode = ChainMode.Singleton;
    }

    /// <summary>
    /// Create a flash image chain
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static new ImageChain CreateFromFile(string filepath)
        => ImageChain.CreateFromFile(filepath);

    /// <summary>
    /// Create a flash image chain from plain base64 <br />
    /// Not incuding the header 'base64://'
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static new ImageChain CreateFromBase64(string base64)
        => ImageChain.CreateFromBase64(base64);

    /// <summary>
    /// Create a flash image chain from url (limit 10MB)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static new ImageChain CreateFromURL(string url)
        => ImageChain.CreateFromURL(url);

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
    internal static new ImageChain Parse(string code)
       => ImageChain.Parse(code);

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

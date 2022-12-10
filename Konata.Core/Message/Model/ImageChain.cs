using System;
using System.IO;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;
using Konata.Core.Events.Model;
using Konata.Core.Utils.FileFormat;
using Konata.Core.Utils.Network;

// ReSharper disable InvertIf
// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Message.Model;

public class ImageChain : BaseChain
{
    /// <summary>
    /// Image Url
    /// </summary>
    public string ImageUrl { get; protected set; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; protected set; }

    /// <summary>
    /// File hash
    /// </summary>
    public string FileHash { get; protected set; }

    /// <summary>
    /// MD5 byte[]
    /// </summary>
    public byte[] HashData { get; protected set; }

    /// <summary>
    /// Image data
    /// </summary>
    public byte[] FileData { get; protected set; }

    /// <summary>
    /// Image data length
    /// </summary>
    public uint FileLength { get; protected set; }

    /// <summary>
    /// Image width
    /// </summary>
    public uint Width { get; protected set; }

    /// <summary>
    /// Image height
    /// </summary>
    public uint Height { get; protected set; }

    /// <summary>
    /// Image type
    /// </summary>
    public ImageType ImageType { get; protected set; }

    /// <summary>
    /// PicUp information
    /// </summary>
    internal PicUpInfo PicUpInfo { get; private set; }

    protected ImageChain() : base(ChainType.Image, ChainMode.Multiple)
    {
    }

    private ImageChain(string url, string fileName,
        string fileHash, uint width, uint height, uint length, ImageType type)
        : base(ChainType.Image, ChainMode.Multiple)
    {
        ImageUrl = url;
        FileName = fileName;
        FileHash = fileHash;
        FileLength = length;
        Width = width;
        Height = height;
        ImageType = type;

        // UnHex
        HashData = ByteConverter.UnHex(fileHash);
    }

    private ImageChain(byte[] data, uint width,
        uint height, byte[] md5, string md5Str, ImageType type)
        : base(ChainType.Image, ChainMode.Multiple)
    {
        FileData = data;
        FileLength = (uint) data.Length;
        Width = width;
        Height = height;
        HashData = md5;
        FileHash = md5Str;
        ImageType = type;
        FileName = md5Str;
    }

    /// <summary>
    /// Set PicUp info
    /// </summary>
    /// <param name="info"></param>
    internal void SetPicUpInfo(PicUpInfo info)
    {
        PicUpInfo = info;

        // Set cached info
        if (info.UseCached)
        {
            Width = info.CachedInfo.Width;
            Height = info.CachedInfo.Height;
            HashData = info.CachedInfo.Hash;
            ImageType = info.CachedInfo.Type;
            FileLength = info.CachedInfo.Length;
        }
    }

    /// <summary>
    /// Create an image chain
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fileName"></param>
    /// <param name="fileHash"></param>
    /// <param name="length"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static ImageChain Create(string url, string
        fileName, string fileHash, uint width, uint height, uint length, ImageType type)
        => new(url, fileName, fileHash, width, height, length, type);

    /// <summary>
    /// Create an image chain
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static ImageChain Create(byte[] image)
    {
        // Image size limitation
        if (image.Length > 1048576 * 30) return null;

        // Detect image type
        if (FileFormat.DetectImage(image, out var type,
                out var width, out var height))
        {
            // Image type
            var imageType = type switch
            {
                FileFormat.ImageFormat.Jpg => ImageType.Jpg,
                FileFormat.ImageFormat.Png => ImageType.Png,
                FileFormat.ImageFormat.Gif => ImageType.Gif,
                FileFormat.ImageFormat.Bmp => ImageType.Bmp,
                FileFormat.ImageFormat.Webp => ImageType.Webp,
                _ => throw new NotImplementedException(),
            };

            // Image MD5
            var imageMd5 = new Md5Cryptor().Encrypt(image);
            var imageMd5Str = ByteConverter.Hex(imageMd5).ToUpper();

            // Create chain
            return new ImageChain(image, width,
                height, imageMd5, imageMd5Str, imageType);
        }

        return null;
    }

    /// <summary>
    /// Create an image chain
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static ImageChain CreateFromFile(string filepath)
    {
        // File not exist
        if (!File.Exists(filepath))
            throw new FileNotFoundException(filepath);

        return Create(File.ReadAllBytes(filepath));
    }

    /// <summary>
    /// Create an image chain from plain base64 <br />
    /// Not including the header 'base64://'
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static ImageChain CreateFromBase64(string base64)
        => Create(ByteConverter.UnBase64(base64));

    /// <summary>
    /// Create an image chain from url (limit 10MB)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static ImageChain CreateFromUrl(string url)
        => Create(Http.Get(url, limitLen: 1048576 * 10).Result);

    /// <summary>
    /// Set image url
    /// </summary>
    /// <param name="url"></param>
    internal void SetImageUrl(string url)
        => ImageUrl = url;

    /// <summary>
    /// Set image data
    /// </summary>
    /// <param name="data"></param>
    internal void SetImageData(byte[] data)
        => FileData = data;
    
    /// <summary>
    /// Parse the code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static ImageChain Parse(string code)
    {
        var args = GetArgs(code);
        {
            var file = args["file"];

            // Create from url
            if (file.StartsWith("http://")
                || file.StartsWith("https://"))
            {
                return CreateFromUrl(file);
            }

            // Create from base64
            if (file.StartsWith("base64://"))
            {
                return CreateFromBase64(file[9..file.Length]);
            }

            // Create from local file
            if (File.Exists(file))
            {
                return CreateFromFile(file);
            }

            // Create from hash
            if (file.Length == 32)
            {
                var width = uint.Parse(args["width"]);
                var height = uint.Parse(args["height"]);
                var length = uint.Parse(args["length"]);
                var type = (ImageType) uint.Parse(args["type"]);

                return Create(file, file, file, width, height, length, type);
            }

            // Ignore
            return null;
        }
    }

    public override string ToString()
        => $"[KQ:image," +
           $"file={FileName}," +
           $"width={Width}," +
           $"height={Height}," +
           $"length={FileLength}," +
           $"type={(int) ImageType}]";

    internal override string ToPreviewString()
        => "[图片]";
}

/// <summary>
/// Image type
/// </summary>
public enum ImageType
{
    Invalid = 1,
    Face = 4,
    Jpg = 1000,
    Png = 1001,
    Webp = 1002,
    Pjpeg = 1003,
    Sharpp = 1004,
    Bmp = 1005,
    Gif = 2000,
    Apng = 2001
}

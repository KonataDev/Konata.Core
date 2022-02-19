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

namespace Konata.Core.Message.Model
{
    public class ImageChain : BaseChain
    {
        /// <summary>
        /// Image Url
        /// </summary>
        public string ImageUrl { get; private set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// File hash
        /// </summary>
        public string FileHash { get; }

        /// <summary>
        /// MD5 byte[]
        /// </summary>
        public byte[] HashData { get; private set; }

        /// <summary>
        /// Image data
        /// </summary>
        public byte[] FileData { get; }

        /// <summary>
        /// Image data length
        /// </summary>
        public uint FileLength { get; private set; }

        /// <summary>
        /// Image width
        /// </summary>
        public uint Width { get; private set; }

        /// <summary>
        /// Image height
        /// </summary>
        public uint Height { get; private set; }

        /// <summary>
        /// Image type
        /// </summary>
        internal ImageType ImageType { get; private set; }

        /// <summary>
        /// Picup information
        /// </summary>
        internal PicUpInfo PicUpInfo { get; private set; }

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

            // Unhash
            HashData = ByteConverter.UnHex(fileHash);
        }

        private ImageChain(byte[] data, uint width,
            uint height, byte[] md5, string md5str, ImageType type)
            : base(ChainType.Image, ChainMode.Multiple)
        {
            FileData = data;
            FileLength = (uint) data.Length;
            Width = width;
            Height = height;
            HashData = md5;
            FileHash = md5str;
            ImageType = type;
            FileName = md5str;
        }

        /// <summary>
        /// Set PicUp info
        /// </summary>
        /// <param name="info"></param>
        public void SetPicUpInfo(PicUpInfo info)
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
        internal static ImageChain Create(string url,
            string fileName, string fileHash, uint width,
            uint height, uint length, ImageType type)
        {
            return new(url, fileName, fileHash, width, height, length, type);
        }

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
                    FileFormat.ImageFormat.JPG => ImageType.JPG,
                    FileFormat.ImageFormat.PNG => ImageType.PNG,
                    FileFormat.ImageFormat.GIF => ImageType.GIF,
                    FileFormat.ImageFormat.BMP => ImageType.BMP,
                    FileFormat.ImageFormat.WEBP => ImageType.WEBP,
                    _ => throw new NotImplementedException(),
                };

                // Image MD5
                var imageMD5 = new Md5Cryptor().Encrypt(image);
                var imageMD5Str = ByteConverter.Hex(imageMD5).ToUpper();

                // Create chain
                return new ImageChain(image, width,
                    height, imageMD5, imageMD5Str, imageType);
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
        /// Not incuding the header 'base64://'
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
        public static ImageChain CreateFromURL(string url)
            => Create(Utils.Network.Http.Get(url, limitLen: 1048576 * 10).Result);

        /// <summary>
        /// Set image url
        /// </summary>
        /// <param name="url"></param>
        internal void SetImageUrl(string url)
            => ImageUrl = url;

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
                    return CreateFromURL(file);
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
    }

    public enum ImageType
    {
        INVALID = 1,
        FACE = 4,
        JPG = 1000,
        PNG = 1001,
        WEBP = 1002,
        PJPEG = 1003,
        SHARPP = 1004,
        BMP = 1005,
        GIF = 2000,
        APNG = 2001
    }
}

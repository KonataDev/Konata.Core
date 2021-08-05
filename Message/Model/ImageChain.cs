using System;
using System.IO;

using Konata.Utils.IO;
using Konata.Utils.Crypto;
using Konata.Core.Utils.Format;

namespace Konata.Core.Message.Model
{
    public class ImageChain : BaseChain
    {
        public string ImageUrl { get; }

        public string FileName { get; }

        public string FileHash { get; }

        /// <summary>
        /// MD5 byte[]
        /// </summary>
        public byte[] HashData { get; }

        /// <summary>
        /// Image data
        /// </summary>
        public byte[] FileData { get; }

        /// <summary>
        /// Image width
        /// </summary>
        public uint Width { get; }

        /// <summary>
        /// Image height
        /// </summary>
        public uint Height { get; }

        /// <summary>
        /// Image type
        /// </summary>
        internal ImageType ImageType { get; }

        private ImageChain(string url, string filename, string filehash)
            : base(ChainType.Image)
        {
            ImageUrl = url;
            FileName = filename;
            FileHash = filehash;
        }

        private ImageChain(byte[] data, uint width,
            uint height, byte[] md5, string md5str, ImageType type)
            : base(ChainType.Image)
        {
            FileData = data;
            Width = width;
            Height = height;
            HashData = md5;
            FileHash = md5str;
            ImageType = type;
            FileName = md5str;
        }

        /// <summary>
        /// Create an image chain
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <param name="filehash"></param>
        /// <returns></returns>
        internal static ImageChain Create(string url,
            string filename, string filehash)
        {
            return new(url, filename, filehash);
        }

        /// <summary>
        /// Create an image chain
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageChain Create(byte[] image)
        {
            // Detect image type
            if (Image.Detect(image, out var type,
                out var width, out var height))
            {
                // Image type
                var imageType = type switch
                {
                    0 => ImageType.JPG,
                    1 => ImageType.PNG,
                    2 => ImageType.GIF,
                    3 => ImageType.BMP,
                    4 => ImageType.WEBP,
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
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageChain Create(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }

            return Create(File.ReadAllBytes(filepath));
        }

        /// <summary>
        /// Parse code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static BaseChain Parse(string code)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
            => $"[KQ:image,file={FileName}]";
    }

    public enum ImageType : int
    {
        FACE = 4,
        JPG = 1000,
        PNG = 1001,
        WEBP = 1002,
        BMP = 1005,
        GIF = 2000,
    }
}

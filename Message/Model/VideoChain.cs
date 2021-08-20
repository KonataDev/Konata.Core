// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model
{
    public class VideoChain : BaseChain
    {
        public string VideoUrl { get; }

        public string FileName { get; }

        public string FileHash { get; }

        public string StoragePath { get; set; }

        /// <summary>
        /// Video frame width
        /// </summary>
        public uint Width { get; private set; }

        /// <summary>
        /// Video frame height
        /// </summary>
        public uint Height { get; private set; }

        /// <summary>
        /// Video duration
        /// </summary>
        public uint Duration { get; }

        private VideoChain(string fileName, string fileHash,
            string storagePath, uint frameWidth, uint frameHeight, uint duration)
            : base(ChainType.Video, ChainMode.Singleton)
        {
            FileName = fileName;
            FileHash = fileHash;
            Width = frameWidth;
            Height = frameHeight;
            Duration = duration;
            StoragePath = storagePath;
        }

        /// <summary>
        /// Create a video chain
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileHash"></param>
        /// <param name="storagePath"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        internal static VideoChain Create(string fileName, string fileHash,
            string storagePath, uint frameWidth, uint frameHeight, uint duration)
        {
            return new(fileName, fileHash, storagePath, frameWidth, frameHeight, duration);
        }

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static VideoChain Parse(string code)
        {
            return null;
        }

        public override string ToString()
            => $"[KQ:video," +
               $"file={FileName}," +
               $"width={Width}," +
               $"height={Height}]";
    }
}

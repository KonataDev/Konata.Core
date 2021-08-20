// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        public string RecordUrl { get; }

        public string FileName { get; }

        public string FileHash { get; }

        private RecordChain(string url, string fileName, string fileHash)
            : base(ChainType.Record, ChainMode.Singleton)
        {
            RecordUrl = url;
            FileName = fileName;
            FileHash = fileHash;
        }

        /// <summary>
        /// Create a record chain
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="fileHash"></param>
        /// <returns></returns>
        internal static RecordChain Create(string url,
            string fileName, string fileHash)
        {
            return new(url, fileName, fileHash);
        }

        /// <summary>
        /// Parse the code to a chain
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RecordChain Parse(string code)
        {
            return null;
        }

        public override string ToString()
            => $"[KQ:record," +
               $"file={FileName}]";
    }
}

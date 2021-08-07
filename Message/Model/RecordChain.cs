using System;

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        public string FileName { get; set; }

        private RecordChain(string fileName)
            : base(ChainType.Record)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Create a record chain
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static RecordChain Create(string fileName)
        {
            return new(fileName);
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
            => $"[KQ:record]"; // TODO: record code
    }
}

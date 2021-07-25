using System;

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        public string FileName { get; set; }

        public RecordChain()
            => Type = ChainType.Record;
    }
}

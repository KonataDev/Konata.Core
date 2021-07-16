using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        public string FileName { get; set; }

        public RecordChain()
            => Type = ChainType.Record;

        public override string ToString()
            => $"[KQ:record,file={FileName}]";
    }
}

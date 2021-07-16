using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message.Model
{
    public class PlainTextChain : BaseChain
    {
        public string Content { get; set; }

        public PlainTextChain()
            => Type = ChainType.Text;

        public override string ToString()
            => Content;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message.Model
{
    public class PlainTextChain : MessageChain
    {
        public string Content { get; set; }

        public PlainTextChain()
            => Type = ChainType.Text;

        public override string ToString()
            => Content;
    }
}

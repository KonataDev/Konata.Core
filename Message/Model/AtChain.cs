using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message.Model
{
    public class AtChain : MessageChain
    {
        public uint AtUin { get; set; }

        public AtChain()
            => Type = ChainType.At;

        public override string ToString()
            => $"[KQ:at,qq={(AtUin == 0 ? "all" : AtUin.ToString())}]";
    }
}

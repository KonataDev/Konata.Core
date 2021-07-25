using System;

namespace Konata.Core.Message.Model
{
    public class AtChain : BaseChain
    {
        public uint AtUin { get; set; }

        public AtChain()
            => Type = ChainType.At;

        public AtChain(uint uin)
        {
            AtUin = uin;
            Type = ChainType.At;
        }
    }
}

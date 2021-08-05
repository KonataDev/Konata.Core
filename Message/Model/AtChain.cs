using System;

namespace Konata.Core.Message.Model
{
    public class AtChain : BaseChain
    {
        public uint AtUin { get; }

        private AtChain(uint uin)
            : base(ChainType.At)
        {
            AtUin = uin;
        }

        /// <summary>
        /// Create an at chain
        /// </summary>
        /// <param name="memberUin"></param>
        /// <returns></returns>
        public static AtChain Create(uint memberUin)
        {
            return new(memberUin);
        }

        internal static BaseChain Parse(string code)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
            => $"[KQ:at,qq={(AtUin == 0 ? "all" : AtUin.ToString())}]";
    }
}

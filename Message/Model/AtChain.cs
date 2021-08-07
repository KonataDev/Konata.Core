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

        /// <summary>
        /// Parse the code to a chain
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static BaseChain Parse(string code)
        {
            var args = GetArgs(code);
            {
                return Create(uint.Parse(args["qq"]));
            }
        }

        public override string ToString()
            => $"[KQ:at,qq={(AtUin == 0 ? "all" : AtUin.ToString())}]";
    }
}

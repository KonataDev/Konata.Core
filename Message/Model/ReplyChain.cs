// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Message.Model
{
    public class ReplyChain : BaseChain
    {
        public uint ReplyUin { get; }

        public uint SourceId { get; }

        private ReplyChain(uint sourceId, uint replyUin)
            : base(ChainType.Reply, ChainMode.Singletag)
        {
            SourceId = sourceId;
            ReplyUin = replyUin;
        }

        /// <summary>
        /// Create a reply chain
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="replyUin"></param>
        /// <returns></returns>
        internal static ReplyChain Create(uint sourceId, uint replyUin)
        {
            return new(sourceId, replyUin);
        }

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static ReplyChain Parse(string code)
        {
            var args = GetArgs(code);
            {
                var qq = uint.Parse(args["qq"]);
                var source = uint.Parse(args["id"]);

                return Create(source, qq);
            }
        }

        public override string ToString()
            => $"[KQ:reply," +
               $"qq={ReplyUin}," +
               $"id={SourceId}]";
    }
}

using System;

namespace Konata.Core.Message.Model
{
    public class PlainTextChain : BaseChain
    {
        public string Content { get; }

        private PlainTextChain(string content)
            : base(ChainType.Text)
        {
            Content = content;
        }

        /// <summary>
        /// Create a text chain
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static PlainTextChain Create(string text)
        {
            return new(text);
        }

        internal static BaseChain Parse(string code)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
            => Content;
    }
}

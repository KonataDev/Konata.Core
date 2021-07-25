using System;

namespace Konata.Core.Message.Model
{
    public class PlainTextChain : BaseChain
    {
        public string Content { get; set; }

        public PlainTextChain()
            => Type = ChainType.Text;

        public PlainTextChain(string content)
        {
            Content = content;
            Type = ChainType.Text;
        }
    }
}

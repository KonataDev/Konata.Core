using System;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model
{
    public class XmlChain : BaseChain
    {
        /// <summary>
        /// Xml content
        /// </summary>
        public string Content { get; }

        private XmlChain(string xml)
            : base(ChainType.Xml, ChainMode.Singleton)
        {
            Content = xml;
        }

        /// <summary>
        /// Create a xml chain
        /// </summary>
        /// <param name="xml"></param>
        public static XmlChain Create(string xml)
        {
            return new(xml);
        }

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static XmlChain Parse(string code)
        {
            var args = GetArgs(code);
            {
                return Create(UnEscape(args["content"]));
            }
        }

        public override string ToString()
            => $"[KQ:xml,content={Escape(Content)}]";
    }
}

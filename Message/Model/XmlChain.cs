namespace Konata.Core.Message.Model
{
    public class XmlChain : BaseChain
    {
        private string Content { get; }

        private XmlChain(string xml)
            : base(ChainType.Xml, ChainMode.Singleton)
        {
            Content = xml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        internal XmlChain Create(string xml)
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
            return null;
        }

        public override string ToString()
            => $"[KQ:xml,content={Content}]";
    }
}

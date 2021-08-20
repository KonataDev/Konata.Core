namespace Konata.Core.Message.Model
{
    public class JsonChain : BaseChain
    {
        private string Content { get; }

        private JsonChain(string json)
            : base(ChainType.Json, ChainMode.Singleton)
        {
            Content = json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        internal JsonChain Create(string json)
        {
            return new(json);
        }

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static JsonChain Parse(string code)
        {
            return null;
        }

        public override string ToString()
            => $"[KQ:json,content={Content}]";
    }
}

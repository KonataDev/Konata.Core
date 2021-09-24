using System;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message.Model
{
    public class JsonChain : BaseChain
    {
        /// <summary>
        /// Json content
        /// </summary>
        private string Content { get; }

        private JsonChain(string json)
            : base(ChainType.Json, ChainMode.Singleton)
        {
            Content = json;
        }

        /// <summary>
        /// Create a json chain
        /// </summary>
        /// <param name="json"></param>
        public static JsonChain Create(string json)
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
            => $"[KQ:json,content={Escape(Content)}]";
    }
}

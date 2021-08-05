using System;

namespace Konata.Core.Message
{
    public abstract class BaseChain
    {
        public enum ChainType
        {
            At,         // At
            Text,       // Ttext
            Image,      // Image
            Record,     // Record
            //Emoji,      // QQ Emoji
            QFace,      // QQ Face
        }

        public ChainType Type { get; set; }

        protected BaseChain(ChainType type)
            => Type = type;
    }
}

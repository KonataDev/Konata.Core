using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message
{
    public class MessageChain
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
    }
}

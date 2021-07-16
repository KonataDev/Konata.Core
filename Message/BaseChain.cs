using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Message
{
    public class BaseChain
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

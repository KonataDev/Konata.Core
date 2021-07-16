using System;
using System.Collections.Generic;

using Konata.Core.Message.Model;

namespace Konata.Core.Message
{
    public class MessageChain : List<BaseChain>
    {
        public override string ToString()
        {
            var content = "";
            foreach (var element in this)
            {
                content += element.ToString();
            }

            return content;
        }

        public class Builder
        {
            private MessageChain _chain;

            public Builder()
            {

            }

            public MessageChain Build()
                => _chain;

            //public Builder Image()
            //{
            //    _chain.Add(new ImageChain());
            //    return this;
            //}

            public Builder PlainText(string message)
            {
                _chain.Add(new PlainTextChain { Content = message });
                return this;
            }

            public Builder At(uint uin)
            {
                _chain.Add(new AtChain { AtUin = uin });
                return this;
            }

            public Builder QFace(uint faceId)
            {
                _chain.Add(new QFaceChain { FaceId = faceId });
                return this;
            }

            //public Builder Record()
            //{

            //}
        }
    }
}

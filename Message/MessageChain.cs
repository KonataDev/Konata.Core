using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Konata.Core.Message.Model;

namespace Konata.Core.Message
{
    public class MessageChain
    {
        public List<BaseChain> Chains
            => _chains;

        public int Count
            => _chains.Count;

        private List<BaseChain> _chains;

        public MessageChain()
        {
            _chains = new();
        }

        /// <summary>
        /// Add a new chain
        /// </summary>
        /// <param name="chain"></param>
        public void Add(BaseChain chain)
           => _chains.Add(chain);

        /// <summary>
        /// Convert a text message to chain
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static MessageChain Eval(string message)
        {
            var builder = new MessageBuilder();
            {
                var regexp = new Regex
                    (@"\[KQ:(at|image|record|qface).*?\]");

                // Match pattern
                var matches = regexp.Matches(message);

                if (matches.Count != 0)
                {
                    int textIndex = 0;

                    // Process every code
                    foreach (Match i in matches)
                    {
                        if (i.Index != textIndex)
                        {
                            builder.PlainText(message[textIndex..i.Index]);
                        }

                        // Convert the code to chain
                        var chain = i.Groups[1].Value switch
                        {
                            "at" => AtChain.Parse(i.Value),
                            "image" => ImageChain.Parse(i.Value),
                            "qface" => QFaceChain.Parse(i.Value),
                            "record" => RecordChain.Parse(i.Value),
                            _ => null,
                        };

                        // Add new chain
                        if (chain != null)
                        {
                            builder.Add(chain);
                        }

                        // Update index
                        textIndex = i.Index + i.Length;
                    }

                    // Process the suffix
                    if (textIndex != message.Length)
                    {
                        builder.PlainText(message[textIndex..message.Length]);
                    }
                }

                // No code included
                else
                {
                    builder.PlainText(message);
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Convert chain to code string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var content = "";
            foreach (var element in Chains)
            {
                content += element.ToString();
            }

            return content;
        }
    }

    public class MessageBuilder
    {
        private MessageChain _chain;

        public MessageBuilder()
        {
            _chain = new();
        }

        public MessageChain Build()
            => _chain;

        /// <summary>
        /// Add a chain
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public MessageBuilder Add(BaseChain chain)
        {
            _chain.Add(chain);
            return this;
        }

        /// <summary>
        /// Plain text
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public MessageBuilder PlainText(string message)
        {
            _chain.Add(PlainTextChain.Create(message));
            return this;
        }

        /// <summary>
        /// At chain
        /// </summary>
        /// <param name="uin"></param>
        /// <returns></returns>
        public MessageBuilder At(uint uin)
        {
            _chain.Add(AtChain.Create(uin));
            return this;
        }

        /// <summary>
        /// Qface chain
        /// </summary>
        /// <param name="faceId"></param>
        /// <returns></returns>
        public MessageBuilder QFace(uint faceId)
        {
            _chain.Add(QFaceChain.Create(faceId));
            return this;
        }

        /// <summary>
        /// Image chain
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public MessageBuilder Image(byte[] data)
        {
            _chain.Add(ImageChain.Create(data));
            return this;
        }

        /// <summary>
        /// Image chain
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MessageBuilder Image(string filePath)
        {
            _chain.Add(ImageChain.CreateFromFile(filePath));
            return this;
        }

        /// <summary>
        /// Record chain
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MessageBuilder Record(string filePath)
        {
            //if (RecordChain.Create(filePath, out var chain))
            //{
            //    _chain.Add(chain);
            //}

            return this;
        }
    }
}

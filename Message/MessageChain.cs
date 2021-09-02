using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Konata.Core.Message.Model;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message
{
    public class MessageChain
    {
        internal List<BaseChain> Chains
            => _chains;

        private readonly List<BaseChain> _chains;

        internal MessageChain()
            => _chains = new();

        internal MessageChain(params BaseChain[] chain)
            => _chains = new(chain.Where(i => i != null));

        internal void Add(BaseChain chain)
            => _chains.Add(chain);

        internal void AddRange(IEnumerable<BaseChain> chains)
            => _chains.AddRange(chains);

        /// <summary>
        /// Convert chain to code string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Chains.Aggregate("", (current, element) => current + element);

        /// <summary>
        /// Find chain
        /// </summary>
        /// <typeparam name="TChain"></typeparam>
        /// <returns></returns>
        public List<TChain> FindChain<TChain>()
            => Chains.Where(i => i is TChain).Cast<TChain>().ToList();

        /// <summary>
        /// Get a chain
        /// </summary>
        /// <typeparam name="TChain"></typeparam>
        /// <returns></returns>
        public TChain GetChain<TChain>()
            => FindChain<TChain>().FirstOrDefault();

        public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainType type)
            => x.Chains.Where(c => c.Type != type);

        public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainType type)
            => x.Chains.Where(c => c.Type == type);

        public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainMode mode)
            => x.Chains.Where(c => c.Mode != mode);

        public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainMode mode)
            => x.Chains.Where(c => c.Mode == mode);

        public BaseChain this[int index]
            => _chains[index];

        public List<BaseChain> this[Type type]
            => Chains.Where(c => c.GetType() == type).ToList();

        public List<BaseChain> this[BaseChain.ChainMode mode]
            => Chains.Where(c => c.Mode == mode).ToList();

        public List<BaseChain> this[BaseChain.ChainType type]
            => Chains.Where(c => c.Type == type).ToList();
    }

    public class MessageBuilder
    {
        private readonly MessageChain _chain;

        public MessageBuilder()
        {
            _chain = new();
        }

        /// <summary>
        /// Build a message chain
        /// </summary>
        /// <returns></returns>
        public MessageChain Build()
        {
            // Scan chains
            foreach (var i in _chain.Chains)
            {
                // If found a singleton chain
                if (i.Mode == BaseChain.ChainMode.Singleton)
                {
                    // Then drop other chains
                    return new MessageChain(_chain[BaseChain
                        .ChainMode.Singletag].FirstOrDefault(), i);
                }
            }

            // Return multiple chain
            return _chain;
        }

        /// <summary>
        /// Convert a text message to chain
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static MessageBuilder Eval(string message)
        {
            var builder = new MessageBuilder();
            {
                var regexp = new Regex
                    (@"\[KQ:(at|image|qface|record|video|reply|json|xml).*?\]");

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
                        BaseChain chain = i.Groups[1].Value switch
                        {
                            "at" => AtChain.Parse(i.Value),
                            "image" => ImageChain.Parse(i.Value),
                            "qface" => QFaceChain.Parse(i.Value),
                            "record" => RecordChain.Parse(i.Value),
                            "video" => VideoChain.Parse(i.Value),
                            "reply" => ReplyChain.Parse(i.Value),
                            "json" => JsonChain.Parse(i.Value),
                            "xml" => XmlChain.Parse(i.Value),
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

            return builder;
        }

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
            _chain.Chains.Add(ImageChain.CreateFromFile(filePath));
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

        /// <summary>
        /// Video chain
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MessageBuilder Video(string filePath)
        {
            //if (RecordChain.Create(filePath, out var chain))
            //{
            //    _chain.Add(chain);
            //}
            return this;
        }

        public static MessageBuilder operator +(MessageBuilder x, MessageBuilder y)
        {
            var z = new MessageBuilder();
            {
                z._chain.AddRange(x._chain.Chains);
                z._chain.AddRange(y._chain.Chains);
            }
            return z;
        }
    }
}

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Konata.Core.Message.Model;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message
{
    public class MessageChain : IEnumerable<BaseChain>
    {
        internal List<BaseChain> Chains { get; }

        internal MessageChain()
            => Chains = new();

        internal MessageChain(params BaseChain[] chain)
            => Chains = new(chain.Where(i => i != null));

        /// <summary>
        /// Add chain
        /// </summary>
        /// <param name="chain"></param>
        internal void Add(BaseChain chain)
            => Chains.Add(chain);

        /// <summary>
        /// Add chains
        /// </summary>
        /// <param name="chains"></param>
        internal void AddRange(IEnumerable<BaseChain> chains)
            => Chains.AddRange(chains);

        public IEnumerator<BaseChain> GetEnumerator()
            => Chains.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

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

        public List<BaseChain> this[Range r]
        {
            get
            {
                var (offset, length) = r.GetOffsetAndLength(Chains.Count);
                return Chains.GetRange(offset, length);
            }
        }

        public BaseChain this[int index]
            => Chains[index];

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

        /// <summary>
        /// Create builder
        /// </summary>
        public MessageBuilder()
            => _chain = new();

        /// <summary>
        /// Create builder with chains
        /// </summary>
        /// <param name="chains"></param>
        public MessageBuilder(params BaseChain[] chains)
            => _chain = new(chains);

        /// <summary>
        /// Create builder with chains
        /// </summary>
        /// <param name="chains"></param>
        public MessageBuilder(IEnumerable<BaseChain> chains)
        {
            _chain = new();
            _chain.AddRange(chains);
        }

        /// <summary>
        /// Create builder with an initial string
        /// </summary>
        /// <param name="text"></param>
        public MessageBuilder(string text)
        {
            _chain = new();
            PlainText(text);
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
                    var textIndex = 0;

                    // Process each code
                    foreach (Match i in matches)
                    {
                        if (i.Index != textIndex)
                        {
                            builder.PlainText(message[textIndex..i.Index]);
                        }

                        // Convert the code to a chain
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
                else builder.PlainText(message);
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
        /// Add chains
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public MessageBuilder Add(IEnumerable<BaseChain> chain)
        {
            _chain.AddRange(chain);
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
            _chain.Add(RecordChain.CreateFromFile(filePath));
            return this;
        }

        // /// <summary>
        // /// Video chain
        // /// </summary>
        // /// <param name="filePath"></param>
        // /// <returns></returns>
        //public MessageBuilder Video(string filePath)
        //{
        //    //if (RecordChain.Create(filePath, out var chain))
        //    //{
        //    //    _chain.Add(chain);
        //    //}
        //    return this;
        //}

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

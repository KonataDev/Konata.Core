using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Konata.Core.Message.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Message;

public class MessageBuilder : IEnumerable
{
    private MessageChain _chain;

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
        Text(text);
    }

    public IEnumerator GetEnumerator()
        => _chain.GetEnumerator();
    
    /// <summary>
    /// Build a message chain
    /// </summary>
    /// <returns></returns>
    public MessageChain Build()
    {
        TextChain last = null;
        var chain = new MessageChain();

        // Scan chains
        foreach (var i in _chain.Chains)
        {
            // If found a singleton chain
            if (i.Mode == BaseChain.ChainMode.Singleton)
            {
                // Then drop other chains
                return new MessageChain(_chain[BaseChain.ChainMode.Singletag].FirstOrDefault(), i);
            }

            // Combine text chains
            //////////////////////

            // Just append if not text chain
            if (i.Type != BaseChain.ChainType.Text)
            {
                last = null;
                chain.Add(i);
                continue;
            }

            // Combine with last text chain
            if (last != null) last.Combine((TextChain) i);
            else
            {
                chain.Add(i);

                // Keep last text chain
                last = (TextChain) i;
            }
        }

        return _chain = chain;
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
                (@"\[KQ:(at|image|flash|face|bface|record|video|reply|json|xml).*?\]");

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
                        builder.Text(message[textIndex..i.Index]);
                    }

                    // Convert the code to a chain
                    var chain = i.Groups[1].Value switch
                    {
                        "at" => AtChain.Parse(i.Value),
                        "image" => ImageChain.Parse(i.Value),
                        "flash" => FlashImageChain.Parse(i.Value),
                        "face" => QFaceChain.Parse(i.Value),
                        "bface" => BFaceChain.Parse(i.Value),
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
                    builder.Text(message[textIndex..message.Length]);
                }
            }

            // No code included
            else builder.Text(message);
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
    public MessageBuilder Text(string message)
    {
        _chain.Add(TextChain.Create(message));
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

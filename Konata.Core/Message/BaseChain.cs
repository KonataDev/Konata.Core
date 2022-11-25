using System.Collections.Generic;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message;

public abstract class BaseChain
{
    public enum ChainType
    {
        At,
        Reply,
        Text,
        Image,
        Flash,
        Record,
        Video,
        QFace,
        BFace,
        Xml,
        MultiMsg,
        Json,
        File,
    }

    public enum ChainMode
    {
        Multiple,
        Singleton,
        Singletag
    }

    public ChainType Type { get; protected set; }

    public ChainMode Mode { get; protected set; }

    protected BaseChain(ChainType type, ChainMode mode)
    {
        Type = type;
        Mode = mode;
    }

    /// <summary>
    /// Serialize to protobuf<br/>
    /// // TODO Serialzie
    /// </summary>
    /// <returns></returns>
    internal virtual ProtoTreeRoot ToProtoBuf() => null;

    /// <summary>
    /// To qq generic string
    /// </summary>
    /// <returns></returns>
    internal abstract string ToPreviewString();
    
    /// <summary>
    /// Get arguments of a code string
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static Dictionary<string, string> GetArgs(string code)
    {
        var kvpair = new Dictionary<string, string>();

        // Split with a comma
        // [KQ:x,x=1,y=2] will becomes
        // "KQ:x" "x=1" "y=2"
        var split = code[..^1].Split(',');
        {
            // Split every kvpair with an equal
            // "KQ:x" ignored
            // "x=1" will becomes "x" "1"
            for (var i = 1; i < split.Length; ++i)
            {
                var eqpair = split[i].Split('=');
                if (eqpair.Length < 2) continue;
                {
                    kvpair.Add(eqpair[0],
                        string.Join("=", eqpair[1..]));
                }
            }

            return kvpair;
        }
    }

    /// <summary>
    /// Escape the string
    /// </summary>
    /// <param name="content"></param>
    /// <param name="comma"></param>
    /// <returns></returns>
    internal static string Escape(string content, bool comma = true)
    {
        var str = content.Replace("&", "&amp;")
            .Replace("[", "&#91;").Replace("]", "&#93;");
        {
            if (comma) str = str.Replace(",", "&#44;");
        }
        return str;
    }

    /// <summary>
    /// UnEscape the string
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    internal static string UnEscape(string content)
        => content.Replace("&amp;", "&").Replace("&#91;", "[")
            .Replace("&#93;", "]").Replace("&#44;", ",");
}

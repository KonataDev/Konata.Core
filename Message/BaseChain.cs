using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message
{
    public abstract class BaseChain
    {
        public enum ChainType
        {
            At,
            Reply,
            Text,
            Image,
            Record,
            Video,
            QFace,
            BFace,
            Xml,
            Json,
        }

        public enum ChainMode
        {
            Multiple,
            Singleton,
            Singletag
        }

        public ChainType Type { get; }

        public ChainMode Mode { get; }

        protected BaseChain(ChainType type, ChainMode mode)
        {
            Type = type;
            Mode = mode;
        }

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
                for (int i = 1; i < split.Length; ++i)
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
}

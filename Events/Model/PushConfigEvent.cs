// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model
{
    public class PushConfigEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Highway host server
        /// </summary>
        internal string HighwayHost { get; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Highway host port
        /// </summary>
        internal int HighwayPort { get; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Highway token
        /// </summary>
        internal byte[] HighwayToken { get; }

        internal PushConfigEvent(string highwayHost,
            int highwayPort, byte[] highwayToken) : base(0)
        {
            HighwayHost = highwayHost;
            HighwayPort = highwayPort;
            HighwayToken = highwayToken;
        }

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <param name="highwayHost"></param>
        /// <param name="highwayPort"></param>
        /// <param name="highwayToken"></param>
        /// <returns></returns>
        internal static PushConfigEvent Push(string highwayHost, int highwayPort,
            byte[] highwayToken) => new(highwayHost, highwayPort, highwayToken);
    }
}

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Konata.Core.Common;

namespace Konata.Core.Events.Model
{
    internal class PushConfigEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Highway host server
        /// </summary>
        public ServerInfo HighwayHost { get; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Highway token
        /// </summary>
        public byte[] HighwayTicket { get; }

        internal PushConfigEvent(string highwayHost,
            ushort highwayPort, byte[] highwayTicket) : base(0)
        {
            HighwayTicket = highwayTicket;
            HighwayHost = new(highwayHost, highwayPort);
        }

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <param name="highwayHost"></param>
        /// <param name="highwayPort"></param>
        /// <param name="highwayTicket"></param>
        /// <returns></returns>
        internal static PushConfigEvent Push(string highwayHost, ushort highwayPort,
            byte[] highwayTicket) => new(highwayHost, highwayPort, highwayTicket);
    }
}

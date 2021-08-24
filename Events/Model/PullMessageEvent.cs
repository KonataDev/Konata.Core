namespace Konata.Core.Events.Model
{
    internal class PullMessageEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Sync cookie <br/>
        /// </summary>
        public byte[] SyncCookie { get; }

        private PullMessageEvent(byte[] syncCookie)
            : base(0, false)
        {
            SyncCookie = syncCookie;
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="syncCookie"></param>
        /// <returns></returns>
        internal static PullMessageEvent Create(byte[] syncCookie)
            => new(syncCookie);
    }
}

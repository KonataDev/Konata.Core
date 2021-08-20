namespace Konata.Core.Events.Model
{
    internal class PrivateMessagePullEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Sync cookie <br/>
        /// </summary>
        public byte[] SyncCookie { get; }

        private PrivateMessagePullEvent(byte[] syncCookie)
            : base(0, false)
        {
            SyncCookie = syncCookie;
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="syncCookie"></param>
        /// <returns></returns>
        internal static PrivateMessagePullEvent Create(byte[] syncCookie)
            => new(syncCookie);
    }
}

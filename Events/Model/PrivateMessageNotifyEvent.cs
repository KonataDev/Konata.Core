namespace Konata.Core.Events.Model
{
    public class PrivateMessageNotifyEvent : ProtocolEvent
    {
        private PrivateMessageNotifyEvent() : base(0)
        {
        }

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <returns></returns>
        internal static PrivateMessageNotifyEvent Push() => new();
    }
}

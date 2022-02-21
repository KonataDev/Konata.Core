namespace Konata.Core.Events.Model
{
    public class FriendMessageNotifyEvent : ProtocolEvent
    {
        private FriendMessageNotifyEvent() : base(0)
        {
        }

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <returns></returns>
        internal static FriendMessageNotifyEvent Push() => new();
    }
}

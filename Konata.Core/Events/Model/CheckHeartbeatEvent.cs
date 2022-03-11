namespace Konata.Core.Events.Model
{
    public class CheckHeartbeatEvent : ProtocolEvent
    {
        private CheckHeartbeatEvent()
            : base(10000, true)
        {
        }

        private CheckHeartbeatEvent(int resultCode)
            : base(resultCode)
        {
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <returns></returns>
        internal static CheckHeartbeatEvent Create()
            => new();

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static CheckHeartbeatEvent Result(int resultCode)
            => new(resultCode);
    }
}

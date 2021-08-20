namespace Konata.Core.Events.Model
{
    public class GroupNewMemberEvent : ProtocolEvent
    {
        private GroupNewMemberEvent(int resultCode)
            : base(resultCode)
        {
        }

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static GroupNewMemberEvent Push(int resultCode)
            => new(resultCode);
    }
}

namespace Konata.Core.Events.Model
{
    public class GroupSpecialTitleEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Group uin <br/>
        /// </summary>
        public uint GroupUin { get; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Member uin <br/>
        /// </summary>
        public uint MemberUin { get; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Title expired time <br/>
        /// </summary>s
        public uint ExpiredTime { get; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Special title <br/>
        /// </summary>
        public string SpecialTitle { get; }

        private GroupSpecialTitleEvent(uint groupUin, uint memberUin,
            string specialTitle, uint expiredTime) : base(2000, true)
        {
            GroupUin = groupUin;
            MemberUin = memberUin;
            SpecialTitle = specialTitle;
            ExpiredTime = expiredTime;
        }

        private GroupSpecialTitleEvent(int resultCode)
            : base(resultCode)
        {
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="specialTitle"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        internal static GroupSpecialTitleEvent Create(uint groupUin, uint memberUin, string specialTitle,
            uint expiredTime) => new(groupUin, memberUin, specialTitle, expiredTime != 0 ? expiredTime : uint.MaxValue);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static GroupSpecialTitleEvent Result(int resultCode)
            => new(resultCode);
    }
}

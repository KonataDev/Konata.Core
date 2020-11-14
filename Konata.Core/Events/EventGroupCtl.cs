using System;

namespace Konata.Events
{
    public class EventGroupCtl : EventParacel
    {
        public enum CtlType
        {
            KickMember,
            MuteMember,
            PromoteAdmin,
            SetSpecialTitle,
            SetGroupCard,
        }

        public CtlType Type { get; set; }

        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// For PromoteAdmin or KickMember <br/>
        /// <b>KickMember</b>: Block the member <br/>
        /// <b>PromoteAdmin</b>: Set or Unset
        /// </summary>
        public bool ToggleType { get; set; }

        /// <summary>
        /// For MuteMember or SetSpecialTitle <br/>
        /// <b>MuteMember</b>: Mute time <br/>
        /// <b>SetSpecialTitle</b>: Title expired time
        /// </summary>
        public uint? TimeSeconds { get; set; }

        /// <summary>
        /// For SetSpecialTitle
        /// </summary>
        public string SpecialTitle { get; set; }

        /// <summary>
        /// For SetGroupCard
        /// </summary>
        public string GroupCard { get; set; }
    }
}

﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model
{
    internal class GroupManagementEvent : ProtocolEvent
    {
        /// <summary>
        /// Dismiss the group
        /// </summary>
        public bool Dismiss { get; }

        /// <summary>
        /// Group code
        /// </summary>
        public ulong GroupCode { get; }

        /// <summary>
        /// Self uin
        /// </summary>
        public uint SelfUin { get; }

        private GroupManagementEvent(ulong groupCode,
            uint selfUin, bool dismiss) : base(6000, true)
        {
            GroupCode = groupCode;
            SelfUin = selfUin;
            Dismiss = dismiss;
        }

        /// <summary>
        /// Construct request event
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="selfUin"></param>
        /// <param name="dismiss"></param>
        /// <returns></returns>
        public static GroupManagementEvent Create(ulong groupCode,
            uint selfUin, bool dismiss) => new(groupCode, selfUin, dismiss);
    }
}

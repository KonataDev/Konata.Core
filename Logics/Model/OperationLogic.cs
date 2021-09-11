using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;
using Konata.Core.Exceptions.Model;

// ReSharper disable UnusedMember.Local
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupKickMemberEvent))]
    [EventSubscribe(typeof(GroupKickMembersEvent))]
    [EventSubscribe(typeof(GroupPromoteAdminEvent))]
    [EventSubscribe(typeof(GroupSpecialTitleEvent))]
    [EventSubscribe(typeof(GroupModifyMemberCardEvent))]
    [EventSubscribe(typeof(GroupMuteMemberEvent))]
    [BusinessLogic("Operation Logic", "Group and friend operations.")]
    public class OperationLogic : BaseLogic
    {
        private const string TAG = "Operation Logic";

        internal OperationLogic(BusinessComponent context)
            : base(context)
        {
        }

        public override void Incoming(ProtocolEvent e)
        {
        }

        /// <summary>
        /// Promote member to admin
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="toggleAdmin"></param>
        /// <returns></returns>
        /// <exception cref="OperationFailedException"></exception>
        public async Task<bool> GroupPromoteAdmin
            (uint groupUin, uint memberUin, bool toggleAdmin)
        {
            var groupInfo = ConfigComponent
                .GetGroupInfo(groupUin);
            {
                // Check owner
                if (groupInfo.OwnerUin != memberUin)
                {
                    throw new OperationFailedException(-1,
                        "Failed to promote admin: You're not the owner of this group.");
                }
            }

            // Promote member to admin
            var result = await GroupPromoteAdmin
                (Context, groupUin, memberUin, toggleAdmin);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to promote admin: Assert failed. Ret => {result.ResultCode}");
                }

                return true;
            }
        }

        /// <summary>
        /// Mute the member in a given group
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="timeSeconds"><b>[In]</b> Mute time. </param>
        /// <exception cref="OperationFailedException"></exception>
        public async Task<bool> GroupMuteMember
            (uint groupUin, uint memberUin, uint timeSeconds)
        {
            var memberInfo = ConfigComponent
                .GetMemberInfo(groupUin, memberUin);
            {
                // Check permission
                if (!memberInfo.IsAdmin)
                {
                    throw new OperationFailedException(-1,
                        "Failed to mute a member: You're not the admin of this group.");
                }
            }

            // Mute a member
            var result = await GroupMuteMember
                (Context, groupUin, memberUin, timeSeconds);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to mute a member: Assert failed. Ret => {result.ResultCode}");
                }

                return true;
            }
        }

        /// <summary>
        /// Kick the member in a given group
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
        public async Task<bool> GroupKickMember
            (uint groupUin, uint memberUin, bool preventRequest)
        {
            var memberInfo = ConfigComponent
                .GetMemberInfo(groupUin, memberUin);
            {
                // Check permission
                if (!memberInfo.IsAdmin)
                {
                    throw new OperationFailedException(-1,
                        "Failed to kick a member: You're not the admin of this group.");
                }
            }

            // Kick a member
            var result = await GroupKickMember
                (Context, groupUin, memberUin, preventRequest);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to kick a member: Assert failed. Ret => {result.ResultCode}");
                }

                return true;
            }
        }

        #region Stub methods

        private static Task<GroupPromoteAdminEvent> GroupPromoteAdmin(BusinessComponent context, uint groupUin, uint memberUin, bool toggleAdmin)
            => context.PostPacket<GroupPromoteAdminEvent>(GroupPromoteAdminEvent.Create(groupUin, memberUin, toggleAdmin));

        private static Task<GroupMuteMemberEvent> GroupMuteMember(BusinessComponent context, uint groupUin, uint memberUin, uint timeSeconds)
            => context.PostPacket<GroupMuteMemberEvent>(GroupMuteMemberEvent.Create(groupUin, memberUin, timeSeconds));

        private static Task<GroupKickMemberEvent> GroupKickMember(BusinessComponent context, uint groupUin, uint memberUin, bool preventRequest)
            => context.PostPacket<GroupKickMemberEvent>(GroupKickMemberEvent.Create(groupUin, memberUin, preventRequest));

        #endregion
    }
}

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
            // Sync the member list
            if (ConfigComponent.IsLackMemberCacheForGroup(groupUin))
                await Context.CacheSync.SyncGroupMemberList(groupUin);

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
            // Sync the member list
            if (ConfigComponent.IsLackMemberCacheForGroup(groupUin))
                await Context.CacheSync.SyncGroupMemberList(groupUin);

            var selfInfo = ConfigComponent.GetMemberInfo(groupUin, Context.Bot.Uin);
            var memberInfo = ConfigComponent.GetMemberInfo(groupUin, memberUin);
            {
                // No permission
                if (selfInfo.Role <= memberInfo.Role)
                {
                    throw new OperationFailedException(-1,
                        $"Failed to mute member: No permission. " +
                        $"{selfInfo.Role} <= {memberInfo.Role}");
                }
            }

            // Mute a member
            var result = await GroupMuteMember
                (Context, groupUin, memberUin, timeSeconds);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to mute member: Assert failed. Ret => {result.ResultCode}");
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
            // Sync the member list
            if (ConfigComponent.IsLackMemberCacheForGroup(groupUin))
                await Context.CacheSync.SyncGroupMemberList(groupUin);

            var selfInfo = ConfigComponent.GetMemberInfo(groupUin, Context.Bot.Uin);
            var memberInfo = ConfigComponent.GetMemberInfo(groupUin, memberUin);
            {
                // No permission
                if (selfInfo.Role <= memberInfo.Role)
                {
                    throw new OperationFailedException(-1,
                        $"Failed to kick member: No permission. " +
                        $"{selfInfo.Role} <= {memberInfo.Role}");
                }
            }

            // Kick a member
            var result = await GroupKickMember
                (Context, groupUin, memberUin, preventRequest);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to kick member: Assert failed. Ret => {result.ResultCode}");
                }

                return true;
            }
        }

        /// <summary>
        /// Set special title
        /// </summary>
        /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
        /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
        /// <param name="specialTitle"><b>[In]</b> Special title. </param>
        /// <param name="expiredTime"><b>[In]</b> Exipred time. </param>
        /// <returns>Return true for operation successfully.</returns>
        /// <exception cref="OperationFailedException"></exception>
        public async Task<bool> GroupSetSpecialTitle(uint groupUin,
            uint memberUin, string specialTitle, uint expiredTime)
        {
            // Sync the member list
            if (ConfigComponent.IsLackMemberCacheForGroup(groupUin))
                await Context.CacheSync.SyncGroupMemberList(groupUin);

            var groupInfo = ConfigComponent.GetGroupInfo(groupUin);
            {
                // No permission
                if (groupInfo.OwnerUin != Context.Bot.Uin)
                {
                    throw new OperationFailedException(-1,
                        "Failed to set special title: Not the owner.");
                }
            }

            // Set special title
            var result = await GroupSetSpecialTitle
                (Context, groupUin, memberUin, specialTitle, expiredTime);
            {
                if (result.ResultCode != 0)
                {
                    throw new OperationFailedException(-2,
                        $"Failed to set special title: Assert failed. Ret => {result.ResultCode}");
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

        private static Task<GroupSpecialTitleEvent> GroupSetSpecialTitle(BusinessComponent context, uint groupUin, uint memberUin, string specialTitle, uint expiredTime)
            => context.PostPacket<GroupSpecialTitleEvent>(GroupSpecialTitleEvent.Create(groupUin, memberUin, specialTitle, expiredTime));

        #endregion
    }
}

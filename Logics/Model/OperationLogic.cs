using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

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

        public async Task<int> GroupPromoteAdmin
            (uint groupUin, uint memberUin, bool toggleAdmin)
        {
            // TODO:
            // Check the permission
            
            var result = await GroupPromoteAdmin
                (Context, groupUin, memberUin, toggleAdmin);

            // TODO:
            // The operation result

            return 0;
        }

        public async Task<int> GroupMuteMember
            (uint groupUin, uint memberUin, uint timeSeconds)
        {
            // TODO:
            // Check the permission
            
            var result = await GroupMuteMember
                (Context, groupUin, memberUin, timeSeconds);

            // TODO:
            // The operation result

            return 0;
        }

        public async Task<int> GroupKickMember
            (uint groupUin, uint memberUin, bool preventRequest)
        {
            // TODO:
            // Check the permission
            
            var result = await GroupKickMember
                (Context, groupUin, memberUin, preventRequest);

            // TODO:
            // The operation result

            return 0;
        }

        #region Stub methods

        private static Task<GroupPromoteAdminEvent> GroupPromoteAdmin(BusinessComponent context, uint groupUin, uint memberUin, bool toggleAdmin)
            => context.PostEvent<PacketComponent, GroupPromoteAdminEvent>(new GroupPromoteAdminEvent {GroupUin = groupUin, MemberUin = memberUin, ToggleType = toggleAdmin});

        private static Task<GroupMuteMemberEvent> GroupMuteMember(BusinessComponent context, uint groupUin, uint memberUin, uint timeSeconds)
            => context.PostEvent<PacketComponent, GroupMuteMemberEvent>(new GroupMuteMemberEvent {GroupUin = groupUin, MemberUin = memberUin, TimeSeconds = timeSeconds});

        private static Task<GroupKickMemberEvent> GroupKickMember(BusinessComponent context, uint groupUin, uint memberUin, bool preventRequest)
            => context.PostEvent<PacketComponent, GroupKickMemberEvent>(new GroupKickMemberEvent {GroupUin = groupUin, MemberUin = memberUin, ToggleType = preventRequest});

        #endregion
    }
}

using System;
using System.Threading.Tasks;

using Konata.Core.Attributes;
using Konata.Core.Components.Model;
using Konata.Core.Events;
using Konata.Core.Events.Model;

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
        internal OperationLogic(BusinessComponent context)
            : base(context) { }

        public override void Incoming(ProtocolEvent e)
        {

        }

        public async Task<int> GroupPromoteAdmin
            (uint groupUin, uint memberUin, bool toggleAdmin)
        {
            var request = new GroupPromoteAdminEvent
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = toggleAdmin
            };

            var result = await Context.PostEvent
                <PacketComponent, GroupPromoteAdminEvent>(request);

            // TODO:
            // The operation result

            return 0;
        }

        public async Task<int> GroupMuteMember
            (uint groupUin, uint memberUin, uint timeSeconds)
        {
            var request = new GroupMuteMemberEvent
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                TimeSeconds = timeSeconds
            };

            var result = await Context.PostEvent
                <PacketComponent, GroupMuteMemberEvent>(request);

            // TODO:
            // The operation result

            return 0;
        }

        public async Task<int> GroupKickMember
            (uint groupUin, uint memberUin, bool preventRequest)
        {
            var request = new GroupKickMemberEvent
            {
                GroupUin = groupUin,
                MemberUin = memberUin,
                ToggleType = preventRequest
            };

            var result = await Context.PostEvent
                <PacketComponent, GroupKickMemberEvent>(request);

            // TODO:
            // The operation result

            return 0;
        }
    }
}

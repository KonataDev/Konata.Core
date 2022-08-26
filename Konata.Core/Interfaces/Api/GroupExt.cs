using System.Collections.Generic;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Interfaces.Api;

public static class GroupExt
{
    /// <summary>
    /// Kick the member in a given group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="preventRequest"><b>[In]</b> Flag to prevent member request or no. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> GroupKickMember(this Bot bot, uint groupUin, uint memberUin, bool preventRequest)
        => bot.BusinessComponent.Operation.GroupKickMember(groupUin, memberUin, preventRequest);

    /// <summary>
    /// Mute the member in a given group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="timeSeconds"><b>[In]</b> Mute time. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> GroupMuteMember(this Bot bot, uint groupUin, uint memberUin, uint timeSeconds)
        => bot.BusinessComponent.Operation.GroupMuteMember(groupUin, memberUin, timeSeconds);

    /// <summary>
    /// Promote the member to admin in a given group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="toggleAdmin"><b>[In]</b> Flag to toggle set or unset. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> GroupPromoteAdmin(this Bot bot, uint groupUin, uint memberUin, bool toggleAdmin)
        => bot.BusinessComponent.Operation.GroupPromoteAdmin(groupUin, memberUin, toggleAdmin);

    /// <summary>
    /// Set special title
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <param name="specialTitle"><b>[In]</b> Special title. </param>
    /// <param name="expiredTime"><b>[In]</b> Expired time. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> GroupSetSpecialTitle(this Bot bot, uint groupUin, uint memberUin, string specialTitle, uint expiredTime)
        => bot.BusinessComponent.Operation.GroupSetSpecialTitle(groupUin, memberUin, specialTitle, expiredTime);

    /// <summary>
    /// Leave group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> GroupLeave(this Bot bot, uint groupUin)
        => bot.BusinessComponent.Operation.GroupLeave(groupUin);

    /// <summary>
    /// Poke Group Member
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    [KonataApi(1)]
    public static Task<bool> SendGroupPoke(this Bot bot, uint groupUin, uint memberUin)
        => bot.BusinessComponent.Operation.GroupPoke(groupUin, memberUin);

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="builder"><b>[In]</b> Message chain builder. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<int> SendGroupMessage(this Bot bot, uint groupUin, MessageBuilder builder)
        => bot.BusinessComponent.Messaging.SendGroupMessage(groupUin, builder.Build());

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="message"><b>[In]</b> Text message. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<int> SendGroupMessage(this Bot bot, uint groupUin, string message)
        => bot.SendGroupMessage(groupUin, new MessageBuilder(message));

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="chains"><b>[In]</b> Message chains. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<int> SendGroupMessage(this Bot bot, uint groupUin, params BaseChain[] chains)
        => bot.SendGroupMessage(groupUin, new MessageBuilder(chains));

    /// <summary>
    /// Send message to the group
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="chain"><b>[In]</b> Message chain. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<int> SendGroupMessage(this Bot bot, uint groupUin, MessageChain chain)
        => bot.SendGroupMessage(groupUin, new MessageBuilder(chain));
    
    /// <summary>
    /// Recall a message
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="message"><b>[In]</b> Message struct. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="MessagingException"></exception>
    [KonataApi(1)]
    public static Task<int> RecallMessage(this Bot bot, MessageStruct message)
        => bot.BusinessComponent.Messaging.RecallMessage(message);

    /// <summary>
    /// Upload the image manually
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="image"><b>[In]</b> Image to upload. </param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> UploadGroupImage(this Bot bot, ImageChain image, uint groupUin)
        => bot.BusinessComponent.Messaging.UploadImage(image, groupUin, true);

    /// <summary>
    /// Get member member list
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin. </param>
    /// <param name="forceUpdate"><b>[In]</b> Force update. </param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    [KonataApi(1)]
    public static Task<IReadOnlyList<BotMember>> GetGroupMemberList(this Bot bot, uint groupUin, bool forceUpdate = false)
        => bot.BusinessComponent.CacheSync.GetGroupMemberList(groupUin, forceUpdate);

    /// <summary>
    /// Get member info
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="memberUin"><b>[In]</b> Member uin</param>
    /// <param name="forceUpdate"><b>[In] [Opt]</b> Force update</param>
    /// <returns></returns>
    /// <exception cref="SyncFailedException"></exception>
    [KonataApi(1)]
    public static Task<BotMember> GetGroupMemberInfo(this Bot bot, uint groupUin, uint memberUin, bool forceUpdate = false)
        => bot.BusinessComponent.CacheSync.GetGroupMemberInfo(groupUin, memberUin, forceUpdate);

    /// <summary>
    /// Process group invitation message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="inviterUin"><b>[In]</b> Inviter uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="GroupInviteEvent.Token"/></param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> ApproveGroupInvitation(this Bot bot, uint groupUin, uint inviterUin, long token)
        => bot.BusinessComponent.Operation.ApproveGroupInvitation(groupUin, inviterUin, token);

    /// <summary>
    /// Process group invitation message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="inviterUin"><b>[In]</b> Inviter uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="GroupInviteEvent.Token"/></param>
    /// <param name="reason"><b>[In] [Opt]</b> The reason string</param>
    /// <param name="preventRequest"><b>[In] [Opt]</b> Prevent this request</param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> DeclineGroupInvitation(this Bot bot, uint groupUin, uint inviterUin, long token, string reason = "", bool preventRequest = false)
        => bot.BusinessComponent.Operation.DeclineGroupInvitation(groupUin, inviterUin, token, reason, preventRequest);
    
    /// <summary>
    /// Process group request join message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="reqUin"><b>[In]</b> Inviter uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="GroupInviteEvent.Token"/></param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> ApproveGroupRequestJoin(this Bot bot, uint groupUin, uint reqUin, long token)
        => bot.BusinessComponent.Operation.ApproveGroupRequestJoin(groupUin, reqUin, token);

    /// <summary>
    /// Process group request join message
    /// </summary>
    /// <param name="bot"><b>[In]</b> <see cref="Bot"/> instance</param>
    /// <param name="groupUin"><b>[In]</b> Group uin</param>
    /// <param name="reqUin"><b>[In]</b> Inviter uin</param>
    /// <param name="token"><b>[In]</b> Request <see cref="GroupInviteEvent.Token"/></param>
    /// <param name="reason"><b>[In] [Opt]</b> The reason string</param>
    /// <param name="preventRequest"><b>[In] [Opt]</b> Prevent this request</param>
    /// <returns></returns>
    [KonataApi(1)]
    public static Task<bool> DeclineGroupRequestJoin(this Bot bot, uint groupUin, uint reqUin, long token, string reason = "", bool preventRequest = false)
        => bot.BusinessComponent.Operation.DeclineGroupRequestJoin(groupUin, reqUin, token, reason, preventRequest);
}

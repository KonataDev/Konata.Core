using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Network;
using Guid = Konata.Core.Utils.Guid;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(GroupKickMemberEvent))]
[EventSubscribe(typeof(GroupKickMembersEvent))]
[EventSubscribe(typeof(GroupPromoteAdminEvent))]
[EventSubscribe(typeof(GroupSpecialTitleEvent))]
[EventSubscribe(typeof(GroupModifyMemberCardEvent))]
[EventSubscribe(typeof(GroupMuteMemberEvent))]
[BusinessLogic("Operation Logic", "Group and friend operations.")]
internal class OperationLogic : BaseLogic
{
    private const string TAG = "Operation Logic";

    internal OperationLogic(BusinessComponent context)
        : base(context)
    {
    }

    public override Task Incoming(ProtocolEvent e)
    {
        return Task.CompletedTask;
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
            if (groupInfo.OwnerUin != Context.Bot.Uin)
            {
                throw new OperationFailedException(-1,
                    "Failed to promote admin: You're not the owner of this group.");
            }
        }

        // Promote member to admin
        var args = GroupPromoteAdminEvent.Create(groupUin, memberUin, toggleAdmin);
        var result = await Context.SendPacket<GroupPromoteAdminEvent>(args);
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
        var args = GroupMuteMemberEvent.Create(groupUin, memberUin, timeSeconds);
        var result = await Context.SendPacket<GroupMuteMemberEvent>(args);
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
        var args = GroupKickMemberEvent.Create(groupUin, memberUin, 0, preventRequest);
        var result = await Context.SendPacket<GroupKickMemberEvent>(args);
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
        var args = GroupSpecialTitleEvent.Create(groupUin, memberUin, specialTitle, expiredTime);
        var result = await Context.SendPacket<GroupSpecialTitleEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-2,
                    $"Failed to set special title: Assert failed. Ret => {result.ResultCode}");
            }

            return true;
        }
    }

    /// <summary>
    /// Leave group
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public async Task<bool> GroupLeave(uint groupUin)
    {
        // Leave group
        var args = GroupLeaveEvent.Create(groupUin, Context.Bot.Uin, false);
        var result = await Context.SendPacket<GroupLeaveEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-2,
                    $"Failed to leave group: Assert failed. Ret => {result.ResultCode}");
            }

            return true;
        }
    }

    /// <summary>
    /// Poke Group Member
    /// </summary>
    /// <param name="groupUin"><b>[In]</b> Group uin being operated. </param>
    /// <param name="memberUin"><b>[In]</b> Member uin being operated. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public async Task<bool> GroupPoke(uint groupUin, uint memberUin)
    {
        var args = GroupPokeEvent.Create(groupUin, memberUin);
        var result = await Context.SendPacket<ProtocolEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    $"Failed to poke member: Assert failed. Ret => {result.ResultCode}");
            }

            return true;
        }
    }

    /// <summary>
    /// Poke Friend
    /// </summary>
    /// <param name="friendUin"><b>[In]</b> Friend uin being operated. </param>
    /// <returns>Return true for operation successfully.</returns>
    /// <exception cref="OperationFailedException"></exception>
    public async Task<bool> FriendPoke(uint friendUin)
    {
        var args = FriendPokeEvent.Create(Context.Bot.Uin, friendUin);
        var result = await Context.SendPacket<ProtocolEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    $"Failed to poke friend: Assert failed. Ret => {result.ResultCode}");
            }

            return true;
        }
    }

    public async Task<bool> ApproveGroupInvitation(uint groupUin, uint inviterUin, long token)
    {
        var args = GroupInviteEvent.Approve(groupUin, inviterUin, token);
        var result = await Context.SendPacket<GroupInviteEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve group invitation: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<bool> DeclineGroupInvitation(uint groupUin, uint inviterUin,
        long token, string reason, bool preventRequest)
    {
        var args = GroupInviteEvent.Decline(groupUin, inviterUin, token, reason, preventRequest);
        var result = await Context.SendPacket<GroupInviteEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve group invitation: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<bool> ApproveGroupRequestJoin(uint groupUin, uint reqUin, long token)
    {
        var args = GroupRequestJoinEvent.Approve(groupUin, reqUin, token);
        var result = await Context.SendPacket<GroupRequestJoinEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve group request: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<bool> DeclineGroupRequestJoin(uint groupUin, uint reqUin,
        long token, string reason, bool preventRequest)
    {
        var args = GroupRequestJoinEvent.Decline(groupUin, reqUin, token, reason, preventRequest);
        var result = await Context.SendPacket<GroupRequestJoinEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve group request: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<bool> ApproveFriendRequest(uint reqUin, long token)
    {
        var args = FriendRequestEvent.Approve(reqUin, token);
        var result = await Context.SendPacket<FriendRequestEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve friend request: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<bool> DeclineFriendRequest(uint reqUin, long token, bool preventRequest)
    {
        var args = FriendRequestEvent.Decline(reqUin, token, preventRequest);
        var result = await Context.SendPacket<FriendRequestEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-1,
                    "Failed to approve group invitation: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return true;
    }

    public async Task<List<ImageOcrResult>> ImageOcr(ImageChain image)
    {
        if (image.ImageUrl == null && image.FileData == null)
        {
            throw new OperationFailedException(-1,
                "Failed to recognize an image: Can not upload image cuz image is empty.");
        }

        // Upload image (optional)
        // We actually can use image url directly to instead
        // of downloading image and upload to ocr server.
        // Idk if we do that can cause what, or a ban.
        var data = image.FileData ?? await Http.Get(image.ImageUrl);
        var guid = Guid.GenerateString();
        var url = await HighwayComponent.ImageOcrUp(
            Context.Bot.Uin,
            ConfigComponent.HighwayConfig.Server,
            ConfigComponent.HighwayConfig.Ticket,
            data, guid
        );

        if (url == null)
        {
            throw new OperationFailedException(-2,
                "Failed to recognize an image: Image upload failed.");
        }

        // Get ocr result
        var args = ImageOcrEvent.Create(image.ImageUrl, image.FileHash, image.Width, image.Height, image.FileLength);
        var result = await Context.SendPacket<ImageOcrEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-3,
                    "Failed to recognize the image: Assert failed. Ret => " + result.ResultCode);
            }
        }

        return result.OcrResult;
    }
    
    public async Task<string> GetOfflineFileUrl(string fileUuid)
    {        
        var args = OfflineFileDownloadEvent.Create(Context.Bot.Uin, fileUuid);
        var result = await Context.SendPacket<OfflineFileDownloadEvent>(args);
        {
            if (result.ResultCode != 0)
            {
                throw new OperationFailedException(-3,
                    "Failed to get the file url: Assert failed. Ret => " + result.ResultCode);
            }
        }
        return result.FileUrl;
    }
}

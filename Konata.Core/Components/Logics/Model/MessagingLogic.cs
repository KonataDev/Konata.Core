using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.IO;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Logics.Model;

[BusinessLogic("Messaging Logic", "Responsible for the core messages.")]
internal class MessagingLogic : BaseLogic
{
    private const string TAG = "Messaging Logic";

    internal MessagingLogic(BusinessComponent context)
        : base(context)
    {
    }

    /// <summary>
    /// Send the message to a friend
    /// </summary>
    /// <param name="friendUin"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="MessagingException"></exception>
    public async Task<bool> SendFriendMessage(uint friendUin, MessageChain message)
    {
        // Wait for upload done
        var results = await UploadResources(friendUin, message, false);

        // Check results
        if (results.Contains(false))
        {
            // Task failed
            throw new MessagingException("Send friend message failed: Task failed.\n" +
                                         $"checkAtChain => {results[0]}, " +
                                         $"uploadImage => {results[1]}, " +
                                         $"uploadRecord => {results[2]}, " +
                                         $"uploadMultiMsg => {results[3]}");
        }

        // Send the message
        var result = await SendFriendMessage(Context, friendUin, message);
        if (result.ResultCode == 0) return true;
        {
            throw new MessagingException("Send friend message failed: " +
                                         $"Assert failed. Ret => {result.ResultCode}");
        }
    }

    /// <summary>
    /// Send the message to a group
    /// </summary>
    /// <param name="groupUin"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="MessagingException"></exception>
    public async Task<bool> SendGroupMessage(uint groupUin, MessageChain message)
    {
        // Wait for upload done
        var results = await UploadResources(groupUin, message, true);

        // Check results
        if (results.Contains(false))
        {
            // Some task failed
            throw new MessagingException("Send group message failed: Task failed.\n" +
                                         $"checkAtChain => {results[0]}, " +
                                         $"uploadImage => {results[1]}, " +
                                         $"uploadRecord => {results[2]}, " +
                                         $"uploadMultiMsg => {results[3]}");
        }

        // Send the message
        var result = await SendGroupMessage(Context, groupUin, message);
        if (result.ResultCode == 0) return true;
        {
            throw new MessagingException("Send group message failed: " +
                                         $"Assert failed. Ret => {result.ResultCode}");
        }
    }

    #region Resource upload logics

    /// <summary>
    /// Upload resources
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="message"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    /// <exception cref="FailedToUploadException"></exception>
    private async Task<bool[]> UploadResources(uint uin, MessageChain message, bool isGroup)
    {
        var atChains = new List<AtChain>();
        var imageChains = new List<ImageChain>();
        var recordChains = new List<RecordChain>();
        var sideMultiMsgs = new List<MultiMsgChain>();
        MultiMsgChain mainMultiMsg = null;

        // Recursively enumerate chains
        // to collect resources for uploading
        void CollectResources(MessageChain msgs)
        {
            foreach (var item in msgs)
            {
                switch (item)
                {
                    case AtChain at:
                        atChains.Add(at);
                        break;

                    case ImageChain img:
                        imageChains.Add(img);
                        break;

                    case RecordChain rec:
                        recordChains.Add(rec);
                        break;

                    case MultiMsgChain mul:

                        // ref the first multi msg chain
                        if (mainMultiMsg == null) mainMultiMsg = mul;

                        // side multimsgs
                        else sideMultiMsgs.Add(mul);

                        // Continue enumeration
                        foreach (var i in mul.Messages) CollectResources(i.Chain);

                        break;
                }
            }
        }

        // Collect resources
        CollectResources(message);

        try
        {
            // Batch up
            var tasks = await Task.WhenAll(
                // Replace at chain display name
                SearchAt(atChains, uin, isGroup),

                // Upload images
                UploadImages(imageChains, uin, isGroup),

                // Upload records
                UploadRecords(recordChains, uin, isGroup),

                // Placeholder for multisg
                Task.FromResult(false)
            );

            // Upload multi message
            tasks[^1] = mainMultiMsg == null
                        || await UploadMultiMsg(mainMultiMsg, sideMultiMsgs, uin, isGroup);

            return tasks;
        }
        catch (Exception e)
        {
            // oops
            throw new FailedToUploadException(e);
        }
    }

    /// <summary>
    /// Search at
    /// </summary>
    /// <param name="chains"></param>
    /// <param name="uin"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    private async Task<bool> SearchAt(List<AtChain> chains, uint uin, bool isGroup)
    {
        // Ignore
        if (!isGroup) return true;

        // Find the at chains
        foreach (var i in chains)
        {
            // If uin is zero
            // meant to ping all the members
            if (i.AtUin == 0) i.DisplayString = "@全体成员";

            // None zero,
            // Then check the relationship
            else
            {
                // Okay we've got it
                if (ConfigComponent.TryGetMemberInfo
                        (uin, i.AtUin, out var member))
                {
                    i.DisplayString = $"@{member.NickName}";
                }

                // F! We might have to pull
                // more member data from server
                else
                {
                    // Check if lacks the member cache
                    if (ConfigComponent.IsLackMemberCacheForGroup(uin))
                    {
                        // Pull the cache and try again
                        if (await Context.CacheSync.SyncGroupMemberList(uin))
                        {
                            // Okay try again
                            i.DisplayString = ConfigComponent.TryGetMemberInfo
                                (uin, i.AtUin, out member)
                                ? $"@{member.NickName}"
                                : $"@{i.AtUin}";
                        }

                        // F? Sync failed
                        else i.DisplayString = $"@{i.AtUin}";
                    }

                    // F? The wrong user
                    else i.DisplayString = $"@{i.AtUin}";
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Upload image manually
    /// </summary>
    /// <param name="image"></param>
    /// <param name="uin"></param>
    /// <param name="c2c"></param>
    /// <returns></returns>
    public async Task<bool> UploadImage(ImageChain image, uint uin, bool c2c)
    {
        var images = new List<ImageChain> {image};
        var result = await UploadImages(images, uin, c2c);
        {
            if (!result) return false;
            {
                // Maybe need ImageStore.down
                // in the future.
                image.SetImageUrl($"https://gchat.qpic.cn/gchatpic_new/0/0-0-{image.FileHash}/0");
                return true;
            }
        }
    }

    /// <summary>
    /// Upload images
    /// </summary>
    /// <param name="image"></param>
    /// <param name="uin"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    private async Task<bool> UploadImages(List<ImageChain> image, uint uin, bool isGroup)
    {
        // 1. Request ImageStore.GroupPicUp/OffPicUp
        // 2. Upload the image via highway
        // 3. Return false while failed to upload

        // Return true if no image to load
        if (image.Count <= 0) return true;

        var blocks = image.Slices(20);
        foreach (var item in blocks)
        {
            var block = item.ToList();

            // Request image upload
            var result = isGroup
                ? await GroupPicUp(Context, uin, block)
                : await OffPicUp(Context, uin, block);

            // Set upload data
            for (var i = 0; i < block.Count; ++i)
                block[i].SetPicUpInfo(result.UploadInfo[i]);

            // Highway image upload
            if (!await HighwayComponent.PicDataUp(Context.Bot.Uin, block, isGroup))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Upload multimsg
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="main"></param>
    /// <param name="sides"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    private async Task<bool> UploadMultiMsg(MultiMsgChain main,
        List<MultiMsgChain> sides, uint uin, bool isGroup)
    {
        // Chain packup
        var packed = MessagePacker.PackMultiMsg(main, sides,
            isGroup ? MessagePacker.Mode.Group : MessagePacker.Mode.Friend);

        // Compressing the data
        if (packed == null) return false;
        packed = Compression.GZip(packed);

        // Request apply up
        var result = await MultiMsgApplyUp(Context, uin, packed);
        if (result.ResultCode != 0) return false;
        {
            // Setup the highway info
            main.SetMultiMsgUpInfo(result.UploadInfo, packed);
        }

        // Highway multimsg upload
        return await HighwayComponent.MultiMsgUp(uin, Context.Bot.Uin, main);
    }

    /// <summary>
    /// Upload the records
    /// </summary>
    /// <param name="message"></param>
    /// <param name="uin"></param>
    /// <param name="isGroup"></param>
    /// <returns></returns>
    private async Task<bool> UploadRecords(List<RecordChain> message, uint uin, bool isGroup)
    {
        // No need to upload
        if (message.Count <= 0) return true;

        // Return false if audio configuration not enabled
        if (!ConfigComponent.GlobalConfig.EnableAudio)
        {
            Context.LogW(TAG, "The audio function is currently disabled. \n" +
                              "Note: This function lack of codec library 'libSilkCodec', " +
                              "Please reference https://github.com/KonataDev/libSilkCodec");
            return false;
        }

        // Check if no highway server
        if (ConfigComponent.HighwayConfig == null)
        {
            Context.LogW(TAG, "Highway server is not present.");
            return false;
        }

        foreach (var i in message)
        {
            // Setup the highway info
            Context.LogV(TAG, "Uploading record file via highway.");
            i.SetPttUpInfo(Context.Bot.Uin, new PttUpInfo
            {
                Host = ConfigComponent.HighwayConfig.Server.Host,
                Port = ConfigComponent.HighwayConfig.Server.Port,
                UploadTicket = ConfigComponent.HighwayConfig.Ticket,
            });

            // Upload the record
            return await HighwayComponent
                .PttUp(uin, Context.Bot.Uin, i, isGroup);
        }

        Context.LogV(TAG, "Recored uploaded.");
        return true;
    }

    #endregion

    #region Stub methods

    private static void ConfirmReadGroupMessage(BusinessComponent context, GroupMessageEvent e)
        => context.PostPacket(GroupMessageReadEvent.Create(e.GroupUin, e.Message.Sequence, e.SessionSequence));

    private static Task<ProtocolEvent> SendGroupMessage(BusinessComponent context, uint groupUin, MessageChain message)
        => context.SendPacket<ProtocolEvent>(GroupMessageEvent.Create(groupUin, context.Bot.Uin, message));

    private static Task<ProtocolEvent> SendFriendMessage(BusinessComponent context, uint friendUin, MessageChain message)
        => context.SendPacket<ProtocolEvent>(FriendMessageEvent.Create(friendUin, context.Bot.Uin, message));

    private static Task<PicUpEvent> GroupPicUp(BusinessComponent context, uint groupUin, List<ImageChain> images)
        => context.SendPacket<PicUpEvent>(PicUpEvent.GroupUp(groupUin, context.Bot.Uin, images));

    private static Task<PicUpEvent> OffPicUp(BusinessComponent context, uint groupUin, List<ImageChain> images)
        => context.SendPacket<PicUpEvent>(PicUpEvent.OffUp(groupUin, context.Bot.Uin, images));

    private static Task<GroupPttUpEvent> GroupPttUp(BusinessComponent context, uint groupUin, RecordChain record)
        => context.SendPacket<GroupPttUpEvent>(GroupPttUpEvent.Create(groupUin, context.Bot.Uin, record));

    private static Task<MultiMsgApplyUpEvent> MultiMsgApplyUp(BusinessComponent context, uint destUin, byte[] packed)
        => context.SendPacket<MultiMsgApplyUpEvent>(MultiMsgApplyUpEvent.Create(destUin, packed));

    #endregion
}

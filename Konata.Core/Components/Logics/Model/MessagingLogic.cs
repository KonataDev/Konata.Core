using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Exceptions.Model;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Utils;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Network;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(GroupMessageEvent))]
[BusinessLogic("Messaging Logic", "Responsible for the core messages.")]
internal class MessagingLogic : BaseLogic
{
    private const string TAG = "Messaging Logic";

    internal MessagingLogic(BusinessComponent context)
        : base(context)
    {
    }

    public override Task Incoming(ProtocolEvent e)
    {
        // Forward messages to userend
        Context.PostEventToEntity(e);
        return Task.CompletedTask;
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
        // Wait for tasks done
        var results = await Task.WhenAll(
            SearchImageAndUpload(friendUin, message, false)
        );

        // Check results
        if (results.Contains(false))
        {
            // Task failed
            throw new MessagingException("Send friend message failed: Task failed.\n" +
                                         $"uploadImage => {results[0]}\n");
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
        // Wait for tasks done
        var results = await Task.WhenAll(
            SearchImageAndUpload(groupUin, message, true),
            SearchRecordAndUpload(groupUin, message),
            SearchMultiMsgAndUpload(groupUin, message),
            SearchAt(groupUin, message)
        );

        // Check results
        if (results.Contains(false))
        {
            // Some task failed
            throw new MessagingException("Send group message failed: Task failed.\n" +
                                         $"uploadImage => {results[0]}, " +
                                         $"uploadRecord => {results[1]}, " +
                                         $"uploadMultiMsg => {results[2]}, " +
                                         $"checkAtChain => {results[3]}");
        }

        // Send the message
        var result = await SendGroupMessage(Context, groupUin, message);
        if (result.ResultCode == 0) return true;
        {
            throw new MessagingException("Send group message failed: " +
                                         $"Assert failed. Ret => {result.ResultCode}");
        }
    }

    /// <summary>
    /// Search at
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task<bool> SearchAt(uint uin, MessageChain message)
    {
        // Find the at chains
        foreach (var i in message.Chains)
        {
            // Process the relationship
            // between group and member
            if (i.Type != BaseChain.ChainType.At) continue;
            var chain = (AtChain) i;

            // If uin is zero
            // meant to ping all the members
            if (chain.AtUin == 0)
            {
                chain.DisplayString = "@全体成员";
            }

            // None zero,
            // Then check the relationship
            else
            {
                // Okay we've got it
                if (ConfigComponent.TryGetMemberInfo
                        (uin, chain.AtUin, out var member))
                {
                    chain.DisplayString = $"@{member.NickName}";
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
                            chain.DisplayString = ConfigComponent.TryGetMemberInfo
                                (uin, chain.AtUin, out member)
                                ? $"@{member.NickName}"
                                : $"@{chain.AtUin}";
                        }

                        // F? Sync failed
                        else chain.DisplayString = $"@{chain.AtUin}";
                    }

                    // F? The wrong user
                    else chain.DisplayString = $"@{chain.AtUin}";
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Upload image manually
    /// </summary>
    /// <returns></returns>
    public async Task<bool> UploadImage(ImageChain image, bool c2c, uint uin)
    {
        var images = new List<ImageChain> {image};
        var result = await UploadImages(images, c2c, uin);
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
    /// <param name="c2c"></param>
    /// <param name="uin"></param>
    /// <returns></returns>
    private async Task<bool> UploadImages(List<ImageChain> image, bool c2c, uint uin)
    {
        // 1. Request ImageStore.GroupPicUp
        // 2. Upload the image via highway
        // 3. Return false while failed to upload

        if (c2c)
        {
            // Request image upload
            var result = await GroupPicUp(Context, uin, image);
            {
                // Set upload data
                for (var i = 0; i < image.Count; ++i)
                {
                    image[i].SetPicUpInfo(result.UploadInfo[i]);
                }
            }

            // Highway image upload
            return await HighwayComponent
                .GroupPicUp(Context.Bot.Uin, image);
        }
        else
        {
            // TODO:
            // Off picup

            // var result = await PrivateOffPicUp(Context, uin, upload);

            // Image upload for private messages
            return await HighwayComponent.OffPicUp();
        }
    }

    /// <summary>
    /// Upload multimsgs
    /// </summary>
    /// <param name="uin"></param>
    /// <param name="upload"></param>
    /// <returns></returns>
    private async Task<bool> UploadMultiMsgs(uint uin, MultiMsgChain upload)
    {
        // Chain packup
        var packed = MessagePacker.PackMultiMsg(upload.Messages);
        if (packed == null) return false;

        // Compressing the data
        packed = Compression.GZip(packed);

        // Request apply up
        var result = await MultiMsgApplyUp(Context, uin, packed);
        if (result.ResultCode != 0) return false;
        {
            // Setup the highway info
            upload.SetMultiMsgUpInfo(result.UploadInfo, packed);
        }

        // Highway multimsg upload
        if (!await HighwayComponent.MultiMsgUp
                (uin, Context.Bot.Uin, upload)) return false;

        string GetPreviewString()
        {
            var preview = "";
            var limit = 4;
            foreach (var msgstu in upload.Messages)
            {
                if (--limit < 0) break;
                preview += "<title size=\"26\" color=\"#777777\" maxLines=\"2\" lineSpace=\"12\">" +
                           $"{msgstu.Sender.Name}: {msgstu.Chain[0]?.ToPreviewString()}</title>";
            }

            return preview;
        }

        // Update chain
        upload.SetGuid(Guid.Generate());
        upload.SetContent(
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +

            // Msg
            "<msg serviceID=\"35\" templateID=\"1\" action=\"viewMultiMsg\" brief=\"[聊天记录]\" " +
            $"m_resid=\"{upload.MultiMsgUpInfo.MsgResId}\" " +
            $"m_fileName=\"{upload.Guid}\" tSum=\"1\" sourceMsgId=\"0\" " +
            "url=\"\" flag=\"3\" adverSign=\"0\" multiMsgFlag=\"0\">" +

            // Message preview
            "<item layout=\"1\" advertiser_id=\"0\" aid=\"0\">" +
            "<title size=\"34\" maxLines=\"2\" lineSpace=\"12\">转发的聊天记录</title>" +
            GetPreviewString() +
            "<hr hidden=\"false\" style=\"0\" />" +
            $"<summary size=\"26\" color=\"#777777\">查看{upload.Messages.Count}条转发消息</summary>" +
            "</item>" +

            // Banner
            "<source name=\"聊天记录\" icon=\"\" action=\"\" appid=\"-1\" />" +
            "</msg>"
        );

        return true;
    }

    /// <summary>
    /// Search image and upload
    /// </summary>
    /// <param name="uin"><b>[In]</b> Uin</param>
    /// <param name="message"><b>[In]</b> The message chain</param>
    /// <param name="c2c"><b>[In]</b> Group or Private </param>
    private async Task<bool> SearchImageAndUpload(uint uin, MessageChain message, bool c2c)
    {
        // Find the image chain
        var upload = message.FindChain<ImageChain>();
        {
            // No image
            if (upload.Count <= 0) return true;
            return await UploadImages(upload, c2c, uin);
        }
    }

    /// <summary>
    /// Upload the records
    /// </summary>
    /// <returns></returns>
    private async Task<bool> SearchRecordAndUpload(uint uin, MessageChain message)
    {
        // Find the record chain
        var upload = message.GetChain<RecordChain>();
        {
            // No records
            if (upload == null) return true;

            // Return false if audio configuration not enabled
            if (!ConfigComponent.GlobalConfig.EnableAudio)
            {
                Context.LogW(TAG, "The audio function is currently disabled. " +
                                  "Lack of codec library.");
                return false;
            }

            // Upload record via highway
            if (ConfigComponent.HighwayConfig != null)
            {
                // Setup the highway info
                Context.LogV(TAG, "Uploading record file via highway.");
                upload.SetPttUpInfo(Context.Bot.Uin, new PttUpInfo
                {
                    Host = ConfigComponent.HighwayConfig.Server.Host,
                    Port = ConfigComponent.HighwayConfig.Server.Port,
                    UploadTicket = ConfigComponent.HighwayConfig.Ticket,
                });

                // Upload the record
                return await HighwayComponent
                    .GroupPttUp(uin, Context.Bot.Uin, upload);
            }

            // Upload record via http
            Context.LogV(TAG, "Uploading record file via http.");
            var result = await GroupPttUp(Context, uin, upload);
            var retdata = await Http.Post($"http://{result.UploadInfo.Host}" +
                                          $":{result.UploadInfo.Port}/", upload.FileData,
                // Request header
                new Dictionary<string, string>
                {
                    {"User-Agent", $"QQ/{AppInfo.AppBuildVer} CFNetwork/1126"},
                    {"Net-Type", "Wifi"}
                },

                // Search params
                new Dictionary<string, string>
                {
                    {"ver", "4679"},
                    {"ukey", ByteConverter.Hex(result.UploadInfo.Ukey)},
                    {"filekey", result.UploadInfo.FileKey},
                    {"filesize", upload.FileLength.ToString()},
                    {"bmd5", upload.FileHash},
                    {"mType", "pttDu"},
                    {"voice_encodec", $"{(int) upload.RecordType}"}
                });

            // Set upload info
            upload.SetPttUpInfo(Context.Bot.Uin, result.UploadInfo);

            Context.LogV(TAG, "Recored uploaded.");
            Context.LogV(TAG, ByteConverter.Hex(retdata));
            return true;
        }
    }

    /// <summary>
    /// Upload the multimsg
    /// </summary>
    /// <returns></returns>
    private async Task<bool> SearchMultiMsgAndUpload(uint uin, MessageChain message)
    {
        // Find the multimsg chain
        var upload = message.GetChain<MultiMsgChain>();
        {
            // No multimsg
            if (upload == null) return true;

            // Upload
            foreach (var msgstu in upload.Messages)
            {
                // Wait for tasks done
                var results = await Task.WhenAll(
                    SearchImageAndUpload(uin, msgstu.Chain, true),
                    SearchRecordAndUpload(uin, msgstu.Chain),
                    SearchMultiMsgAndUpload(uin, msgstu.Chain),
                    SearchAt(uin, msgstu.Chain)
                );

                // Check results
                if (results.Contains(false))
                {
                    // Some task failed
                    throw new MessagingException("Send group message failed: Task failed.\n" +
                                                 $"uploadImage => {results[0]}, " +
                                                 $"uploadRecord => {results[1]}, " +
                                                 $"uploadMultiMsg => {results[2]}, " +
                                                 $"checkAtChain => {results[3]}");
                }
            }

            // Upload
            return await UploadMultiMsgs(uin, upload);
        }
    }

    #region Stub methods

    private static void ConfirmReadGroupMessage(BusinessComponent context, GroupMessageEvent e)
        => context.SendPacket(GroupMessageReadEvent.Create(e.GroupUin, e.Message.Sequence, e.SessionSequence));

    private static Task<ProtocolEvent> SendGroupMessage(BusinessComponent context, uint groupUin, MessageChain message)
        => context.SendPacket<ProtocolEvent>(GroupMessageEvent.Create(groupUin, context.Bot.Uin, message));

    private static Task<ProtocolEvent> SendFriendMessage(BusinessComponent context, uint friendUin, MessageChain message)
        => context.SendPacket<ProtocolEvent>(FriendMessageEvent.Create(friendUin, context.Bot.Uin, message));

    private static Task<GroupPicUpEvent> GroupPicUp(BusinessComponent context, uint groupUin, List<ImageChain> images)
        => context.SendPacket<GroupPicUpEvent>(GroupPicUpEvent.Create(groupUin, context.Bot.Uin, images));

    private static Task<GroupPttUpEvent> GroupPttUp(BusinessComponent context, uint groupUin, RecordChain record)
        => context.SendPacket<GroupPttUpEvent>(GroupPttUpEvent.Create(groupUin, context.Bot.Uin, record));

    private static Task<LongConnOffPicUpEvent> LongConnOffPicUp(BusinessComponent context, uint friendUin, List<ImageChain> images)
        => context.SendPacket<LongConnOffPicUpEvent>(LongConnOffPicUpEvent.Create(context.Bot.Uin, images));

    private static Task<MultiMsgApplyUpEvent> MultiMsgApplyUp(BusinessComponent context, uint destUin, byte[] packed)
        => context.SendPacket<MultiMsgApplyUpEvent>(MultiMsgApplyUpEvent.Create(destUin, packed));

    #endregion
}

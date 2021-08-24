using System.Threading.Tasks;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Message;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Components.Model;
using Konata.Core.Utils.IO;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(GroupMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageEvent))]
    [EventSubscribe(typeof(PrivateMessageNotifyEvent))]
    [BusinessLogic("Messaging Logic", "Responsible for the core messages.")]
    public class MessagingLogic : BaseLogic
    {
        private const string TAG = "Messaging Logic";

        internal MessagingLogic(BusinessComponent context)
            : base(context)
        {
        }

        public override void Incoming(ProtocolEvent e)
        {
            switch (e)
            {
                // Pull new private message
                case PrivateMessageNotifyEvent:
                    PullPrivateMessage(Context, ConfigComponent.SyncCookie);
                    return;

                // Received a private message
                case PrivateMessageEvent friend:
                    SyncPrivateCookie(friend);
                    break;

                // Received a group message
                case GroupMessageEvent group:
                    ConfirmReadGroupMessage(Context, group);
                    break;
            }

            // Forward messages to userend
            Context.PostEventToEntity(e);
        }

        /// <summary>
        /// Send the message to a friend
        /// </summary>
        /// <param name="friendUin"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> SendPrivateMessage(uint friendUin, MessageChain message)
        {
            // Check and process some resources
            var uploadImage = SearchImageAndUpload(friendUin, message, false);

            // Wait for tasks done
            var results = await Task.WhenAll(uploadImage);
            {
                // Check results
                if (!results[0])
                {
                    // Some tasks failed
                    return -1;
                }
            }

            // Send the message
            return (await SendPrivateMessage
                (Context, friendUin, message)).ResultCode;
        }

        /// <summary>
        /// Send the message to a group
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> SendGroupMessage(uint groupUin, MessageChain message)
        {
            // Check and process some resources
            var uploadImage = SearchImageAndUpload(groupUin, message, true);
            var uploadRecord = SearchRecordAndUpload(groupUin, message);
            var checkAtChain = SearchAt(groupUin, message);

            // Wait for tasks done
            var results = await Task.WhenAll
                (uploadImage, uploadRecord, checkAtChain);
            {
                // Check results
                if (!(results[0] && results[1] && results[2]))
                {
                    // Some tasks failed
                    return -1;
                }
            }

            // Send the message
            return (await SendGroupMessage
                (Context, groupUin, message)).ResultCode;
        }

        /// <summary>
        /// Cearch at
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
                if (i.Type == BaseChain.ChainType.At)
                {
                    var chain = (AtChain) i;

                    // If uin is zero
                    // meant to ping all members
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
                                if (await Context.SyncGroupMemberList(uin))
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
            }

            return true;
        }

        /// <summary>
        /// Upload the image
        /// </summary>
        /// <param name="uin"><b>[In]</b> Uin</param>
        /// <param name="message"><b>[In]</b> The message chain</param>
        /// <param name="c2c"><b>[In]</b> Group or Private </param>
        private async Task<bool> SearchImageAndUpload
            (uint uin, MessageChain message, bool c2c)
        {
            // Find the image chain
            var upload = message.FindChain<ImageChain>();
            {
                // No images
                if (upload.Count <= 0) return true;

                // 1. Request ImageStore.GroupPicUp
                // 2. Upload the image via highway
                // 3. Return false while failed to upload

                if (c2c)
                {
                    // Request image upload
                    var result = await GroupPicUp(Context, uin, upload);
                    {
                        // Set upload data
                        for (var i = 0; i < upload.Count; ++i)
                        {
                            upload[i].SetPicUpInfo(result.UploadInfo[i]);
                        }
                    }

                    // Highway image upload
                    return await Context.HighwayComponent.UploadGroupImages
                        (Context.Bot.Uin, upload.ToArray(), result.UploadInfo.ToArray());
                }
                else
                {
                    // TODO:
                    // Off picup

                    // var result = await PrivateOffPicUp(Context, uin, upload);

                    // Image upload for private messages
                    return await Context.HighwayComponent.UploadPrivateImages();
                }
            }
        }

        /// <summary>
        /// Upload the records
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SearchRecordAndUpload(uint uin, MessageChain message)
        {
            // Return false if audio configuration not enabled
            if (!ConfigComponent.GlobalConfig.EnableAudio) return false;

            

            return false;
        }

        /// <summary>
        /// Update the local sync cookie
        /// </summary>
        /// <param name="e"></param>
        private void SyncPrivateCookie(PrivateMessageEvent e)
        {
            ConfigComponent.SyncCookie = e.SyncCookie;
            Context.LogI(TAG, $"New cookie synced => {ByteConverter.Hex(e.SyncCookie)}");
        }

        #region Stub methods

        private static void ConfirmReadGroupMessage(BusinessComponent context, GroupMessageEvent e)
            => context.PostPacket(GroupMessageReadEvent.Create(e.GroupUin, e.MessageId, e.SessionSequence));

        private static void PullPrivateMessage(BusinessComponent context, byte[] syncCookie)
            => context.PostPacket(PrivateMessagePullEvent.Create(syncCookie));

        private static Task<GroupMessageEvent> SendGroupMessage(BusinessComponent context, uint groupUin, MessageChain message)
            => context.PostPacket<GroupMessageEvent>(GroupMessageEvent.Create(groupUin, context.Bot.Uin, message));

        private static Task<PrivateMessageEvent> SendPrivateMessage(BusinessComponent context, uint friendUin, MessageChain message)
            => context.PostPacket<PrivateMessageEvent>(PrivateMessageEvent.Create(friendUin, context.Bot.Uin, message));

        private static Task<GroupPicUpEvent> GroupPicUp(BusinessComponent context, uint groupUin, List<ImageChain> images)
            => context.PostPacket<GroupPicUpEvent>(GroupPicUpEvent.Create(groupUin, context.Bot.Uin, images));

        private static Task<LongConnOffPicUpEvent> PrivateOffPicUp(BusinessComponent context, uint friendUin, List<ImageChain> images)
            => context.PostPacket<LongConnOffPicUpEvent>(LongConnOffPicUpEvent.Create(context.Bot.Uin, images));

        #endregion
    }
}

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
                    PullPrivateMessage();
                    return;

                // Received a private message
                case PrivateMessageEvent friend:
                    SyncPrivateCookie(friend);
                    break;

                // Received a group message
                case GroupMessageEvent group:
                    ConfirmReadGroupMessage(group);
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
            // Upload the images
            if (!await CheckImageAndUpload(friendUin, message, false))
            {
                // Templorary return
                return -1;
            }

            // Construct the event
            var request = new PrivateMessageEvent
            {
                Message = message,
                FriendUin = friendUin,
            };

            // Send the message
            return (await SendPrivateMessage(request)).ResultCode;
        }

        /// <summary>
        /// Send the message to a group
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<int> SendGroupMessage(uint groupUin, MessageChain message)
        {
            // Upload the images
            if (!await CheckImageAndUpload(groupUin, message, true))
            {
                return -1;
            }

            // Check the at chain
            if (!await CheckAt(groupUin, message))
            {
                return -2;
            }

            // Construct the event
            var request = new GroupMessageEvent
            {
                Message = message,
                GroupUin = groupUin,
                MemberUin = Context.Bot.Uin
            };

            // Send the message
            return (await SendGroupMessage(request)).ResultCode;
        }

        /// <summary>
        /// Check at
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> CheckAt(uint uin, MessageChain message)
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
                        chain.DisplayString = "@全体成员 ";
                    }

                    // None zero,
                    // Then check the relationship
                    else
                    {
                        // Okay we've got it
                        if (ConfigComponent.TryGetMemberInfo
                            (uin, chain.AtUin, out var member))
                        {
                            chain.DisplayString = $"@{member.NickName} ";
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
                                        ? $"@{member.NickName} "
                                        : $"@{chain.AtUin} ";
                                }

                                // F? Sync failed
                                else chain.DisplayString = $"@{chain.AtUin} ";
                            }

                            // F? The wrong user
                            else chain.DisplayString = $"@{chain.AtUin} ";
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
        private async Task<bool> CheckImageAndUpload
            (uint uin, MessageChain message, bool c2c)
        {
            List<ImageChain> upload = new();

            // Find the image chain
            foreach (var i in message.Chains)
            {
                if (i.Type == BaseChain.ChainType.Image)
                {
                    upload.Add((ImageChain) i);
                }
            }

            // Do upload the image
            if (upload.Count > 0)
            {
                // 1. Request ImageStore.GroupPicUp
                // 2. Upload the image via highway
                // 3. Return false while failed to upload

                if (c2c)
                {
                    var request = new GroupPicUpEvent
                    {
                        Images = upload,
                        GroupUin = uin,
                        MemberUin = Context.Bot.Uin
                    };

                    var result = await Context.PostEvent
                        <PacketComponent, GroupPicUpEvent>(request);

                    // Set upload info
                    for (var i = 0; i < upload.Count; ++i)
                    {
                        upload[i].SetPicUpInfo(result.UploadInfo[i]);
                    }

                    // Image upload for group messages
                    return await Context.HighwayComponent.UploadGroupImages
                        (Context.Bot.Uin, upload.ToArray(), result.UploadInfo.ToArray());
                }
                else
                {
                    var request = new PrivateOffPicUpEvent
                    {
                        Images = upload
                    };

                    // TODO:
                    // Off picup

                    var result = await Context.PostEvent
                        <PacketComponent, PrivateOffPicUpEvent>(request);

                    // Image upload for private messages
                    return await Context.HighwayComponent.UploadPrivateImages();
                }
            }

            // No images
            return true;
        }

        /// <summary>
        /// Confirm the messages
        /// </summary>
        /// <param name="e"></param>
        private void ConfirmReadGroupMessage(GroupMessageEvent e)
        {
            Context.PostEvent<PacketComponent>(new GroupMessageReadEvent
            {
                GroupUin = e.GroupUin,
                RequestId = e.MessageId,
                SessionSequence = e.SessionSequence,
            });
        }

        /// <summary>
        /// Pull the new private message
        /// </summary>
        private void PullPrivateMessage()
            => Context.PostEvent<PacketComponent>(new PrivateMessagePullEvent {SyncCookie = ConfigComponent.SyncCookie});

        private Task<GroupMessageEvent> SendGroupMessage(GroupMessageEvent e)
            => Context.PostEvent<PacketComponent, GroupMessageEvent>(e);

        private Task<GroupMessageEvent> SendPrivateMessage(PrivateMessageEvent e)
            => Context.PostEvent<PacketComponent, GroupMessageEvent>(e);

        /// <summary>
        /// Update the local sync cookie
        /// </summary>
        /// <param name="e"></param>
        private void SyncPrivateCookie(PrivateMessageEvent e)
        {
            ConfigComponent.SyncCookie = e.SyncCookie;
            Context.LogI(TAG, $"New cookie synced => {ByteConverter.Hex(e.SyncCookie)}");
        }
    }
}

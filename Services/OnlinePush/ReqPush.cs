using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Packets.SvcRequest;
using Konata.Utils.IO;

namespace Konata.Core.Services.OnlinePush
{
    [Service("OnlinePush.ReqPush", "Push messages from server")]
    [EventDepends(typeof(GroupMessageRecallEvent))]
    [EventDepends(typeof(GroupSettingsAnonymousEvent))]
    [EventDepends(typeof(GroupNewMemberEvent))]
    [EventDepends(typeof(GroupMuteMemberEvent))]
    [EventDepends(typeof(GroupSettingsFrankSpeekingEvent))]
    [EventDepends(typeof(GroupSettingsDirectMessageEvent))]
    [EventDepends(typeof(GroupSettingsUploadEvent))]
    [EventDepends(typeof(GroupSettingsGroupCompostion))]
    //[EventDepends(typeof(GroupPokeEvent))]
    //[EventDepends(typeof(PrivatePokeEvent))]
    public class ReqPush : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            // Parse push event
            var pushMsg = new SvcReqPushMsg(input.Payload.GetBytes());

            // Convert push event to konata event
            return pushMsg.EventType == PushType.Group ?
                 HandlePushGroupEvent(pushMsg.PushPayload, out output) :
                 HandlePushPrivateEvent(pushMsg.PushPayload, out output);
        }

        /// <summary>
        /// Parse push event for group
        /// </summary>
        /// <param name="pushPayload"></param>
        /// <param name="pushEvent"></param>
        /// <returns></returns>
        private bool HandlePushGroupEvent
            (byte[] pushPayload, out ProtocolEvent pushEvent)
        {
            pushEvent = null;

            uint fromGroup;
            byte messageType;

            uint operateTime;
            uint operatorUin;
            uint affectedUin;

            var buffer = new ByteBuffer(pushPayload);
            {
                buffer.TakeUintBE(out fromGroup);
                buffer.TakeByte(out messageType);

                switch (messageType)
                {
                    // 1 New Member
                    case 0x01:
                        break;

                    // 17 Recall Message
                    case 0x11:
                        break;

                    // 12 Mute
                    case 0x0C:

                        buffer.EatBytes(1);
                        buffer.TakeUintBE(out operatorUin);
                        buffer.TakeUintBE(out operateTime);

                        buffer.EatBytes(2); // 00 01
                        buffer.TakeUintBE(out affectedUin);
                        buffer.TakeUintBE(out var timeSeconds);

                        // Mute all members
                        pushEvent = new GroupMuteMemberEvent
                        {
                            GroupUin = fromGroup,
                            MemberUin = affectedUin,
                            OperatorUin = operatorUin,
                            TimeSeconds = timeSeconds
                        };

                        return true;

                    // 14 Anonymous Settings
                    case 0x0E:
                        break;

                    // 15 Upload File/Album Settings
                    case 0x0F:
                        break;

                    // 16 System Messages
                    case 0x10:
                        break;

                    // 20 Poke
                    case 0x14:
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Parse push event for friend
        /// </summary>
        /// <param name="pushPayload"></param>
        /// <param name="pushEvent"></param>
        /// <returns></returns>
        private bool HandlePushPrivateEvent
            (byte[] pushPayload, out ProtocolEvent pushEvent)
        {
            pushEvent = null;
            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null; newSequence = 0;
            return false;
        }
    }
}

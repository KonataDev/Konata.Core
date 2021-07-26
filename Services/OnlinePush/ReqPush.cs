using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Packets.SvcRequest;
using Konata.Utils.IO;
using Konata.Utils.Protobuf;
using Konata.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Services.OnlinePush
{
    [Service("OnlinePush.ReqPush", "Push messages from server")]
    [EventSubscribe(typeof(GroupMessageRecallEvent))]
    [EventSubscribe(typeof(GroupSettingsAnonymousEvent))]
    [EventSubscribe(typeof(GroupNewMemberEvent))]
    [EventSubscribe(typeof(GroupMuteMemberEvent))]
    [EventSubscribe(typeof(GroupSettingsFrankSpeekingEvent))]
    [EventSubscribe(typeof(GroupSettingsDirectMessageEvent))]
    [EventSubscribe(typeof(GroupSettingsUploadEvent))]
    [EventSubscribe(typeof(GroupSettingsGroupCompostion))]
    [EventSubscribe(typeof(GroupPokeEvent))]
    [EventSubscribe(typeof(PrivatePokeEvent))]
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

            uint operateTime = 0;
            uint operatorUin = 0;
            uint affectedUin = 0;

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

                        var recallSuffix = "";

                        // Length
                        buffer.EatBytes(2);

                        // Get data
                        var recallTree = new ProtoTreeRoot(buffer.TakeAllBytes(out _), true);
                        {
                            var info5A = (ProtoTreeRoot)recallTree.GetLeaf("5A");
                            var info1A = (ProtoTreeRoot)info5A.GetLeaf("1A");
                            var info4A = (ProtoTreeRoot)info5A.GetLeaf("4A");
                            {
                                operatorUin = (uint)info5A.GetLeafVar("08");
                                affectedUin = (uint)info1A.GetLeafVar("30");
                            }

                            recallSuffix = info4A.GetLeafString("12");
                        }

                        pushEvent = new GroupMessageRecallEvent
                        {
                            GroupUin = fromGroup,
                            MemberUin = affectedUin,
                            OperatorUin = operatorUin,
                            RecallSuffix = recallSuffix
                        };

                        return true;

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

                        var actionPrefix = "";
                        var actionSuffix = "";

                        // Length
                        buffer.EatBytes(2);

                        // Decode proto tree
                        var pokeTree = new ProtoTreeRoot(buffer.TakeAllBytes(out _), true);
                        {
                            fromGroup = (uint)pokeTree.GetLeafVar("20");

                            // Find keys
                            var keyValPair = (ProtoTreeRoot)pokeTree.GetLeaf("D201");
                            foreach (ProtoTreeRoot i in keyValPair.GetLeaves("3A"))
                            {
                                // Get key name
                                var key = i.GetLeafString("0A");

                                switch (key)
                                {
                                    case "action_str":
                                        actionPrefix = i.GetLeafString("12");
                                        break;

                                    case "suffix_str":
                                        actionSuffix = i.GetLeafString("12");
                                        break;

                                    case "uin_str1":
                                        operatorUin = uint.Parse(i.GetLeafString("12"));
                                        break;

                                    case "uin_str2":
                                        affectedUin = uint.Parse(i.GetLeafString("12"));
                                        break;
                                }
                            }

                            // Failed to parse
                            if (operatorUin == 0 || affectedUin == 0)
                            {
                                return false;
                            }
                        }

                        // Construct event
                        pushEvent = new GroupPokeEvent()
                        {
                            GroupUin = fromGroup,
                            MemberUin = affectedUin,
                            OperatorUin = operatorUin,
                            ActionPrefix = actionPrefix,
                            ActionSuffix = actionSuffix
                        };

                        return true;
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

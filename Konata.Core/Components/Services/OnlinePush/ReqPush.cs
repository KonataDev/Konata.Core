using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets.SvcPush;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct;
using Konata.Core.Utils.JceStruct.Model;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Components.Services.OnlinePush;

using GroupPushHandler = Dictionary<ushort, Func<BotKeyStore, uint, ByteBuffer, ProtocolEvent>>;
using FriendPushHandler = Dictionary<ushort, Func<BotKeyStore, uint, JStruct, ProtocolEvent>>;

[Service("OnlinePush.ReqPush", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class ReqPush : BaseService<OnlineReqPushEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo, BotKeyStore keystore,
        out OnlineReqPushEvent output, List<ProtocolEvent> extra)
    {
        // Parse push event
        var pushMsg = new SvcReqPushMsg(input.Payload.GetBytes());

        // Process push event
        var result = pushMsg.EventType switch
        {
            PushType.Group => HandlePushGroupEvent(pushMsg.PushPayload, keystore),
            PushType.Friend => HandlePushFriendEvent(pushMsg.FromSource, pushMsg.PushPayload, keystore),
            _ => null
        };

        // Add to extra events
        if (result != null) extra.Add(result);

        // Construct push event
        output = OnlineReqPushEvent.Push(pushMsg.packageRequestId, pushMsg.FromSource,
            pushMsg.Unknown0x1C, pushMsg.SvrIp, pushMsg.Unknown0x8D, pushMsg.Unknown0x32);

        // Set sequence
        output.SetSessionSequence(input.Sequence);

        return true;
    }

    /// <summary>
    /// Parse group push event
    /// </summary>
    /// <param name="pushPayload"></param>
    /// <param name="signInfo"></param>
    /// <returns></returns>
    private ProtocolEvent HandlePushGroupEvent(byte[] pushPayload,
        BotKeyStore signInfo)
    {
        var buffer = new ByteBuffer(pushPayload);
        {
            buffer.TakeUintBE(out var fromGroup);
            buffer.TakeByte(out var messageType);

            // Parse events
            return GroupHandler.ContainsKey(messageType)
                ? GroupHandler[messageType].Invoke(signInfo, fromGroup, buffer)
                : null;
        }
    }

    /// <summary>
    /// Parse friend push event
    /// </summary>
    /// <param name="fromSource"></param>
    /// <param name="pushPayload"></param>
    /// <param name="signInfo"></param>
    /// <returns></returns>
    private ProtocolEvent HandlePushFriendEvent(uint fromSource,
        byte[] pushPayload, BotKeyStore signInfo)
    {
        var buffer = new ByteBuffer(pushPayload);
        {
            var tree = Jce.Deserialize(buffer.GetBytes());
            var messageType = (ushort) tree[0].Number.ValueShort;

            // Parse events
            return FriendHandler.ContainsKey(messageType)
                ? FriendHandler[messageType].Invoke(signInfo, fromSource, tree)
                : null;
        }
    }

    private static readonly FriendPushHandler FriendHandler = new()
    {
        {
            // Friend message recall
            0x8a, (key, src, jce) =>
            {
                // Decode proto tree
                var buf = (byte[]) jce[10].SimpleList;
                var recallTree = ProtoTreeRoot.Deserialize(buf, true);

                // Get data
                var info0A = (ProtoTreeRoot) recallTree.GetLeaf("0A");

                var fromUin = (uint) info0A.GetLeafVar("08");
                var toUin = (uint) info0A.GetLeafVar("10");
                var msgSeq = (uint) info0A.GetLeafVar("18");
                var msgUuid = (long) info0A.GetLeafVar("20");
                var msgTime = (uint) info0A.GetLeafVar("28");
                var msgRand = (uint) info0A.GetLeafVar("30");

                var friendUin = fromUin == key.Account.Uin ? toUin : fromUin;

                // Construct event
                return FriendMessageRecallEvent.Push(friendUin, fromUin, msgSeq, msgRand, msgUuid, msgTime);
            }
        },

        {
            // New friend
            0xb3, (key, src, jce) => null
        },

        {
            // Force update group information
            0x27, (key, src, jce) => null
        },

        {
            // Friend poke
            0x122, (key, src, jce) =>
            {
                var actionPrefix = "";
                var actionSuffix = "";
                var operatorUin = 0U;
                var affectedUin = 0U;

                // Decode proto tree
                var buf = (byte[]) jce[10].SimpleList;
                var pokeTree = ProtoTreeRoot.Deserialize(buf, true);
                {
                    // Find keys
                    foreach (ProtoTreeRoot i in pokeTree.GetLeaves("3A"))
                    {
                        // Get key name
                        var keyName = i.GetLeafString("0A");

                        switch (keyName)
                        {
                            case "action_str":
                                i.TryGetLeafString("12", out actionPrefix);
                                break;

                            case "suffix_str":
                                i.TryGetLeafString("12", out actionSuffix);
                                break;

                            case "uin_str1":
                                operatorUin = uint.Parse(i.GetLeafString("12"));
                                break;

                            case "uin_str2":
                                affectedUin = uint.Parse(i.GetLeafString("12"));
                                break;
                        }
                    }

                    // If no affected uin included
                    if (affectedUin == 0)
                    {
                        affectedUin = key.Account.Uin;
                    }

                    // Failed to parse
                    if (operatorUin == 0 || affectedUin == 0) return null;
                }

                // Construct event
                return FriendPokeEvent.Push(operatorUin, actionPrefix, actionSuffix);
            }
        },

        {
            // Friend typing event
            0x115, (key, src, jce) =>
            {
                // Decode proto tree
                var buf = (byte[]) jce[10].SimpleList;
                var typingTree = ProtoTreeRoot.Deserialize(buf, true);
                var friendUin = (uint) typingTree.GetLeafVar("08");

                return FriendTypingEvent.Push(friendUin);
            }
        }
    };

    private static readonly GroupPushHandler GroupHandler = new()
    {
        {
            // Group recall message
            0x11, (key, src, buf) =>
            {
                uint operatorUin;
                uint affectedUin;
                uint msgSequence;
                uint msgRand;
                uint msgTime;

                // Length
                buf.EatBytes(2);

                // Get data
                var recallTree = new ProtoTreeRoot(buf.TakeAllBytes(out _), true);
                {
                    var info5A = (ProtoTreeRoot) recallTree.GetLeaf("5A");
                    var info1A = (ProtoTreeRoot) info5A.GetLeaf("1A");
                    {
                        operatorUin = (uint) info5A.GetLeafVar("08");
                        affectedUin = (uint) info1A.GetLeafVar("30");
                        msgSequence = (uint) info1A.GetLeafVar("08");
                        msgTime = (uint) info1A.GetLeafVar("10");
                        msgRand = (uint) info1A.GetLeafVar("18");
                    }
                }

                // Construct event
                return GroupMessageRecallEvent.Push(src,
                    operatorUin, affectedUin, msgSequence, msgRand, msgTime);
            }
        },

        {
            // Group mute event 
            0x0C, (key, src, buf) =>
            {
                buf.EatBytes(1);
                buf.TakeUintBE(out var operatorUin);
                buf.TakeUintBE(out _);

                buf.EatBytes(2); // 00 01
                buf.TakeUintBE(out var affectedUin);
                buf.TakeUintBE(out var timeSeconds);

                // Construct event
                return GroupMuteMemberEvent.Push(src,
                    affectedUin, operatorUin, timeSeconds);
            }
        },

        {
            // Group anonymous settings event
            0x0E, (key, src, buf) => null
        },

        {
            // Group poke event
            0x14, (key, src, buf) =>
            {
                var actionPrefix = "";
                var actionSuffix = "";
                var operatorUin = 0U;
                var affectedUin = 0U;
                uint fromGroup;

                // Length
                buf.EatBytes(2);

                // Decode proto tree
                var pokeTree = new ProtoTreeRoot(buf.TakeAllBytes(out _), true);
                {
                    fromGroup = (uint) pokeTree.GetLeafVar("20");

                    // Find keys
                    var keyValPair = (ProtoTreeRoot) pokeTree.GetLeaf("D201");
                    foreach (ProtoTreeRoot i in keyValPair.GetLeaves("3A"))
                    {
                        // Get key name
                        var keyName = i.GetLeafString("0A");

                        switch (keyName)
                        {
                            case "action_str":
                                i.TryGetLeafString("12", out actionPrefix);
                                break;

                            case "suffix_str":
                                i.TryGetLeafString("12", out actionSuffix);
                                break;

                            case "uin_str1":
                                operatorUin = uint.Parse(i.GetLeafString("12"));
                                break;

                            case "uin_str2":
                                affectedUin = uint.Parse(i.GetLeafString("12"));
                                break;
                        }
                    }

                    // If no affected uin included
                    if (affectedUin == 0)
                    {
                        affectedUin = key.Account.Uin;
                    }

                    // Failed to parse
                    if (operatorUin == 0 || affectedUin == 0) return null;
                }

                // Construct event
                return GroupPokeEvent.Push(fromGroup, affectedUin,
                    operatorUin, actionPrefix, actionSuffix);
            }
        }
    };

    // GroupNewMember,
    // GroupSettingsUpload,
    // GroupSettingsFranklySpeeking,
    // GroupSettingsDirectMessage,
    // GroupSettingsAnonymous,
    // GroupSettingsGroupComposition,
}

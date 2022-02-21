using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets.SvcPush;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedParameter.Local
// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Konata.Core.Services.OnlinePush;

using EventHandler = Dictionary<byte, Func<BotKeyStore, uint, ByteBuffer, ProtocolEvent>>;

[Service("OnlinePush.ReqPush", "Push messages from server")]
internal class ReqPush : BaseService<OnlineReqPushEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out OnlineReqPushEvent output)
    {
        // Parse push event
        var pushMsg = new SvcReqPushMsg(input.Payload.GetBytes());

        // Convert push event to konata event
        var innerEvent = pushMsg.EventType switch
        {
            PushType.Group => HandlePushGroupEvent(pushMsg.PushPayload, keystore),
            PushType.Friend => HandlePushFriendEvent(pushMsg.PushPayload, keystore),
            _ => null
        };

        // Construct push event
        output = OnlineReqPushEvent.Push(innerEvent, pushMsg.packageRequestId, pushMsg.Unknown0x02,
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
            return _groupHandler.ContainsKey(messageType)
                ? _groupHandler[messageType].Invoke(signInfo, fromGroup, buffer)
                : null;
        }
    }

    /// <summary>
    /// Parse friend push event
    /// </summary>
    /// <param name="pushPayload"></param>
    /// <param name="signInfo"></param>
    /// <returns></returns>
    private ProtocolEvent HandlePushFriendEvent(byte[] pushPayload, BotKeyStore signInfo)
    {
        // var buffer = new ByteBuffer(pushPayload);
        // {
        //     buffer.TakeUintBE(out var fromGroup);
        //     buffer.TakeByte(out var messageType);
        //
        //     // Parse events
        //     return _friendHandler.ContainsKey(messageType)
        //         ? _friendHandler[messageType].Invoke(signInfo, fromGroup, buffer)
        //         : null;
        // }
        return null;
    }

    private static readonly EventHandler _friendHandler = new()
    {
    };

    private static readonly EventHandler _groupHandler = new()
    {
        {
            // Group recall message
            0x11, (key, src, buf) =>
            {
                var recallSuffix = "";
                uint operatorUin;
                uint affectedUin;

                // Length
                buf.EatBytes(2);

                // Get data
                var recallTree = new ProtoTreeRoot(buf.TakeAllBytes(out _), true);
                {
                    var info5A = (ProtoTreeRoot) recallTree.GetLeaf("5A");
                    var info1A = (ProtoTreeRoot) info5A.GetLeaf("1A");
                    var info4A = (ProtoTreeRoot) info5A.GetLeaf("4A");
                    {
                        operatorUin = (uint) info5A.GetLeafVar("08");
                        affectedUin = (uint) info1A.GetLeafVar("30");
                    }

                    if (info4A.TryGetLeafString("12", out var str))
                    {
                        recallSuffix = str;
                    }
                }

                // Construct event
                return GroupMessageRecallEvent.Push(src,
                    affectedUin, operatorUin, recallSuffix);
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
            0x0E, (key, src, buf) =>
            {
                buf.EatBytes(1);
                buf.TakeUintBE(out var operatorUin);
                buf.TakeUintBE(out var timeSeconds);

                // Failed to parse
                if (operatorUin == 0) return null;

                // Construct event
                return GroupSettingsAnonymousEvent.Push(src, operatorUin, timeSeconds == 0);
            }
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

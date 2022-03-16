using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.OnlinePush;

[EventSubscribe(typeof(GroupMessageEvent))]
[Service("OnlinePush.PbPushGroupMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbPushGroupMsg : BaseService<GroupMessageEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupMessageEvent output)
    {
        var message = GroupMessageEvent.Result(0);
        var root = ProtoTreeRoot.Deserialize(input.Payload, true);
        {
            // Parse message source information
            var sourceRoot = root.PathTo<ProtoTreeRoot>("0A.0A");
            {
                message.SetMemberUin((uint) sourceRoot.GetLeafVar("08"));
                message.SetMessageSequence((uint) sourceRoot.GetLeafVar("28"));
                message.SetMessageTime((uint) sourceRoot.GetLeafVar("30"));
                message.SetMessageUuid((uint) sourceRoot.GetLeafVar("38"));
                message.SetMessageRand((uint) root.PathTo<ProtoVarInt>("0A.1A.0A.0A.18"));

                sourceRoot = sourceRoot.PathTo<ProtoTreeRoot>("4A");
                {
                    message.SetGroupUin((uint) sourceRoot.GetLeafVar("08"));
                    message.SetGroupName(sourceRoot.GetLeafString("42"));

                    // Try get member card
                    if (sourceRoot.TryGetLeafString("22", out var cardText))
                    {
                        message.SetMemberCard(cardText);
                    }
                    else
                    {
                        // This member card contains a color code
                        // We need to ignore this
                        sourceRoot = sourceRoot.PathTo<ProtoTreeRoot>("22");
                        if (sourceRoot.GetLeaves("0A").Count == 2)
                        {
                            message.SetMemberCard(sourceRoot
                                .PathTo<ProtoLengthDelimited>("0A[1].12").ToString());
                        }
                    }
                }
            }

            // Parse message slice information
            var sliceInfoRoot = root.PathTo<ProtoTreeRoot>("0A.12");
            {
                var total = (uint) sliceInfoRoot.GetLeafVar("08");
                var index = (uint) sliceInfoRoot.GetLeafVar("10");
                var id = (uint) sliceInfoRoot.GetLeafVar("18");
                message.SetSliceInfo(SliceControl.Create(total, index, id));
            }

            // Parse message content
            var chains = MessagePacker.UnPack(root
                .PathTo<ProtoTreeRoot>("0A.1A.0A"), MessagePacker.ParseMode.Group);
            {
                message.SetMessage(chains);
                message.SetSessionSequence(input.Sequence);
            }
        }

        output = message;
        return true;
    }
}

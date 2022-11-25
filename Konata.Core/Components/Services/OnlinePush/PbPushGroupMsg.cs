using Konata.Core.Events.Model;
using Konata.Core.Message;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.OnlinePush;

[EventSubscribe(typeof(GroupMessageEvent))]
[Service("OnlinePush.PbPushGroupMsg", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbPushGroupMsg : BaseService<GroupMessageEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out GroupMessageEvent output)
    {
        var message = GroupMessageEvent.Push();
        var source = new MessageStruct(MessageStruct.SourceType.Group);

        message.SetMessageStruct(source);
        message.SetSessionSequence(input.Sequence);

        var root = ProtoTreeRoot.Deserialize(input.Payload, true);
        {
            // Parse message source information
            var sourceRoot = root.PathTo<ProtoTreeRoot>("0A.0A");
            {
                var uin = (uint) sourceRoot.GetLeafVar("08");
                var sequence = (uint) sourceRoot.GetLeafVar("28");
                var time = (uint) sourceRoot.GetLeafVar("30");
                var uuid = sourceRoot.GetLeafVar("38");
                var rand = (uint) root.PathTo<ProtoVarInt>("0A.1A.0A.0A.18");

                source.SetSourceInfo(sequence, rand, time, uuid);
                source.SetSenderUin(uin);

                sourceRoot = sourceRoot.PathTo<ProtoTreeRoot>("4A");
                {
                    source.SetReceiverUin((uint) sourceRoot.GetLeafVar("08"));
                    source.SetReceiverName(sourceRoot.GetLeafString("42"));

                    // Try get member card
                    if (sourceRoot.TryGetLeafString("22", out var cardText))
                    {
                        source.SetSenderName(cardText);
                    }
                    else
                    {
                        // This member card contains a color code
                        // We need to ignore this
                        sourceRoot = sourceRoot.PathTo<ProtoTreeRoot>("22");
                        if (sourceRoot.GetLeaves("0A").Count == 2)
                        {
                            source.SetSenderName(sourceRoot.PathTo
                                <ProtoLengthDelimited>("0A[1].12").ToString());
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
                message.SetSliceInfo(MessageSlice.Create(total, index, id));
            }

            // Parse message content
            var chains = MessagePacker.UnPack(root.PathTo
                <ProtoTreeRoot>("0A.1A"), MessagePacker.Mode.Group);
            {
                source.SetMessage(chains);
            }
        }

        output = message;
        return true;
    }
}

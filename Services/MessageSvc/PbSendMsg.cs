using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PbSendMsg", "Send message")]
    [EventSubscribe(typeof(GroupMessageEvent))]
    public class PbSendMsg : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            var tree = new ProtoTreeRoot
                (input.Payload.GetBytes(), true);
            {
                output = GroupMessageEvent
                    .Result(((int) tree.GetLeafVar("08")));
            }

            return true;
        }

        public bool Build(Sequence sequence, GroupMessageEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var root = new ProtoTreeRoot();
            {
                foreach (var chain in input.Message.Chains)
                {
                    switch (chain)
                    {
                        case PlainTextChain textChain:
                            ConstructPlainTextChain(root, textChain);
                            break;

                        case AtChain atChain:
                            ConstructAtChain(root, atChain);
                            break;

                        case QFaceChain qfaceChain:
                            ConstructQFaceChain(root, qfaceChain);
                            break;

                        case ImageChain imageChain:
                            ConstructImageChain(root, imageChain);
                            break;

                        case RecordChain recordChain:
                            ConstructRecordChain(root, recordChain);
                            break;
                    }
                }
            }

            var readReport = new GroupMsg(input.GroupUin, root);

            if (SSOFrame.Create("MessageSvc.PbSendMsg", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(readReport), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        private static void ConstructPlainTextChain(ProtoTreeRoot root, PlainTextChain chain)
        {
            // @formatter:off
            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("0A", (_) =>
                {
                    _.AddLeafString("0A", chain.Content);
                });
            });
            // @formatter:on
        }

        private static void ConstructAtChain(ProtoTreeRoot root, AtChain chain)
        {
            // Construct parameters
            var data = new ByteBuffer();
            {
                data.PutByte(0x00);
                data.PutUintLE(1);
                data.PutByte((byte) chain.DisplayString.Length);
                data.PutByte((byte) (chain.AtUin == 0 ? 0x01 : 0x00));
                data.PutUintBE(chain.AtUin);
                data.PutShortBE(0x0000);
            }

            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("0A", (_) =>
                {
                    _.AddLeafString("0A", chain.DisplayString);
                    _.AddLeafBytes("1A", data.GetBytes());
                });
            });
        }

        private static void ConstructRecordChain(ProtoTreeRoot root, RecordChain chain)
        {
            root.AddTree("22", (_) =>
            {
                _.AddLeafVar("08", 4);
                _.AddLeafVar("10", chain.SelfUin);
                _.AddLeafBytes("22", chain.HashData);
                _.AddLeafString("2A", chain.FileName);
                _.AddLeafVar("30", chain.FileLength);
                _.AddLeafVar("40", chain.PttUpInfo.UploadId);
                _.AddLeafVar("58", 1);
                _.AddLeafString("9201", chain.PttUpInfo.FileKey);
                _.AddLeafVar("9801", chain.TimeSeconds);
                _.AddLeafVar("E801", 1);
                _.AddTree("F201", __ =>
                {
                    __.AddLeafVar("08", 0);
                    __.AddLeafVar("28", 0);
                    __.AddLeafVar("38", 0);
                });
            });
        }

        private static void ConstructImageChain(ProtoTreeRoot root, ImageChain chain)
        {
            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("42", (_) =>
                {
                    _.AddLeafString("12", chain.FileName);
                    _.AddLeafVar("38", chain.PicUpInfo.Ip);
                    _.AddLeafVar("40", chain.PicUpInfo.UploadId);
                    _.AddLeafVar("48", chain.PicUpInfo.Port);
                    _.AddLeafVar("50", 66);
                    //_.AddLeafString("5A", "e3vEdCESKrkycKTZ"); // TODO: Unknown
                    _.AddLeafVar("60", 1);
                    _.AddLeafBytes("6A", chain.HashData);
                    _.AddLeafVar("A001", (long) chain.ImageType);
                    _.AddLeafVar("B001", chain.Width);
                    _.AddLeafVar("B801", chain.Height);
                    _.AddLeafVar("C001", 200); // TODO: Unknown
                    _.AddLeafVar("C801", chain.FileLength);
                    _.AddLeafVar("D001", 1);
                    _.AddLeafVar("E801", 0);
                    _.AddLeafVar("F001", 0);
                    _.AddTree("9202", __ => __.AddLeafVar("78", 2));
                });
            });
        }

        private static void ConstructQFaceChain(ProtoTreeRoot root, QFaceChain chain)
        {
            // @formatter:off
            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("12", (_) =>
                {
                    _.AddLeafVar("08", chain.FaceId);
                });
            });
            // @formatter:on
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (GroupMessageEvent) input, keystore, device, out newSequence, out output);
    }
}

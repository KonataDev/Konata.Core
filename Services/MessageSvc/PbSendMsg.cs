using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Utils.Protobuf;
using Konata.Core.Attributes;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PbSendMsg", "Send message")]
    [EventDepends(typeof(GroupMessageEvent))]
    public class PbSendMsg : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupMessageEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var root = new ProtoTreeRoot();
            {
                foreach (var chain in input.Message)
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

                        // Not supported yet.
                        case ImageChain imageChain:
                        case RecordChain recordChain:
                            break;
                    }
                }
            }

            var readReport = new GroupMsg(input.GroupUin, root);

            if (SSOFrame.Create("MessageSvc.PbSendMsg", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(readReport), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.Account.Uin, signInfo.Session.D2Token, signInfo.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        private void ConstructPlainTextChain(ProtoTreeRoot root, PlainTextChain chain)
        {
            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("0A", (_) =>
                {
                    _.AddLeafString("0A", chain.Content);
                });
            });
        }

        private void ConstructAtChain(ProtoTreeRoot root, AtChain chain)
        {

        }

        private void ConstructQFaceChain(ProtoTreeRoot root, QFaceChain chain)
        {
            root.AddTree("12", (leaf) =>
            {
                leaf.AddTree("12", (_) =>
                {
                    _.AddLeafVar("08", chain.FaceId);
                });
            });
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (GroupMessageEvent)input, signInfo, device, out newSequence, out output);
    }
}

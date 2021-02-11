using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Message.Model;
using Konata.Core.Packet;
using Konata.Core.Packet.Protobuf;
using Konata.Utils.Protobuf;

namespace Konata.Core.Service.MessageSvc
{
    [Service("MessageSvc.PbSendMsg", "Send message")]
    [EventDepends(typeof(GroupMessageEvent))]
    public class PbSendMsg : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupMessageEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
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
                    signInfo.UinInfo.Uin, signInfo.D2Token, signInfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
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

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
            => Build(sequence, (GroupMessageEvent)input, signInfo, out newSequence, out output);
    }
}

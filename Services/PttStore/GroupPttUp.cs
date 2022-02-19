using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.PttStore
{
    [EventSubscribe(typeof(GroupPttUpEvent))]
    [Service("PttStore.GroupPttUp", "Record upload")]
    public class GroupPttUp : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            var tree = new ProtoTreeRoot
                (input.Payload.GetBytes(), true);
            {
                var uploadInfo = new PttUpInfo();
                var leaf = (ProtoTreeRoot) tree.GetLeaf("2A");
                {
                    uploadInfo.Ip = (uint) leaf.GetLeafVar("28");
                    uploadInfo.Host = NetTool.UintToIPBE(uploadInfo.Ip);
                    uploadInfo.Port = (int) leaf.GetLeafVar("30");
                    uploadInfo.Ukey = leaf.GetLeafBytes("3A");
                    uploadInfo.UploadId = (uint) leaf.GetLeafVar("40");
                    uploadInfo.FileKey = leaf.GetLeafString("5A");
                }

                // Construct event
                output = GroupPttUpEvent.Result(0, uploadInfo);
                return true;
            }
        }

        public bool Build(Sequence sequence, GroupPttUpEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = input.SessionSequence;

            var picupRequest = new GroupPttUpRequest
                (input.GroupUin, input.SelfUin, input.UploadRecord);

            if (SSOFrame.Create("PttStore.GroupPttUp", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(picupRequest), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (GroupPttUpEvent) input, keystore, device, out newSequence, out output);
    }
}

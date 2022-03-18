using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.PttStore;

[EventSubscribe(typeof(GroupPttUpEvent))]
[Service("PttStore.GroupPttUp", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GroupPttUp : BaseService<GroupPttUpEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupPttUpEvent output)
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

    protected override bool Build(int sequence, GroupPttUpEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new GroupPttUpRequest
            (input.GroupUin, input.SelfUin, input.UploadRecord));
        return true;
    }
}

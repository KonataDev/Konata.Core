using System;
using System.Collections.Generic;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Konata.Core.Components.Services.ImgStore;

[EventSubscribe(typeof(GroupPicUpEvent))]
[Service("ImgStore.GroupPicUp", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GroupPicUp : BaseService<GroupPicUpEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupPicUpEvent output)
    {
        var tree = new ProtoTreeRoot(input.Payload.GetBytes(), true);
        {
            var leaves = tree.GetLeaves<ProtoTreeRoot>("1A");

            // Invalid data
            if (leaves.Count <= 0)
            {
                throw new Exception("Data error.");
            }

            var uploadInfo = new List<PicUpInfo>();

            // Enumerate all segments
            foreach (var i in leaves)
            {
                var info = new PicUpInfo();
                var cached = i.GetLeafVar("20") == 1;

                // If use the cache
                // We can do not upload the image again
                if (cached)
                {
                    info.Ip = (uint) i.GetLeafVar("30");
                    info.Host = NetTool.UintToIPBE((uint) i.GetLeafVar("30"));
                    info.Port = (int) i.GetLeafVar("38");
                    info.UploadId = (uint) i.GetLeafVar("48");
                    info.UseCached = true;

                    // Cached info
                    var imginfo = i.GetLeaf<ProtoTreeRoot>("2A");
                    {
                        info.CachedInfo = new CachedPicInfo
                        {
                            Hash = imginfo.GetLeafBytes("0A"),
                            Type = (ImageType) imginfo.GetLeafVar("10"),
                            Length = (uint) imginfo.GetLeafVar("18"),
                            Width = (uint) imginfo.GetLeafVar("20"),
                            Height = (uint) imginfo.GetLeafVar("28"),
                        };
                    }
                }

                // We have to
                // upload the iamge
                else
                {
                    info.Ip = (uint) i.GetLeafVar("30");
                    info.Host = NetTool.UintToIPBE((uint) i.GetLeafVar("30"));
                    info.Port = (int) i.GetLeafVar("38");
                    info.UploadId = (uint) i.GetLeafVar("48");
                    info.UploadTicket = i.GetLeafBytes("42");
                    info.UseCached = false;
                }

                uploadInfo.Add(info);
            }

            // Construct event
            output = GroupPicUpEvent.Result(0, uploadInfo);
            return true;
        }
    }

    protected override bool Build(int sequence, GroupPicUpEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output.PutProtoNode(new GroupPicUpRequest(input.GroupUin, input.SelfUin, input.UploadImages));
        return true;
    }
}

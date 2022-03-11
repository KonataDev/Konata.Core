using System;
using System.Collections.Generic;
using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Packets.Protobuf.Highway.Requests;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Konata.Core.Services.ImgStore
{
    [EventSubscribe(typeof(GroupPicUpEvent))]
    [Service("ImgStore.GroupPicUp", "Image upload")]
    public class GroupPicUp : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            var tree = new ProtoTreeRoot
                (input.Payload.GetBytes(), true);
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

        public bool Build(Sequence sequence, GroupPicUpEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = input.SessionSequence;

            var picupRequest = new GroupPicUpRequest(input.GroupUin, input.SelfUin, input.UploadImages);

            if (SSOFrame.Create("ImgStore.GroupPicUp", PacketType.TypeB,
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
            => Build(sequence, (GroupPicUpEvent) input, keystore, device, out newSequence, out output);
    }
}

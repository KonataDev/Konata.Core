using System;
using System.Collections.Generic;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Message.Model;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.LongConn;

[EventSubscribe(typeof(PicUpEvent))]
[Service("LongConn.OffPicUp", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class OffPicUp : BaseService<PicUpEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out PicUpEvent output)
    {
        var tree = new ProtoTreeRoot(input.Payload.GetBytes(), true);
        {
            var leaves = tree.GetLeaves<ProtoTreeRoot>("12");

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
                var cached = i.GetLeafVar("28") == 1;

                // If use the cache
                // We can do not upload the image again
                if (cached)
                {
                    // info.Ip = (uint) i.GetLeafVar("30");
                    // info.Host = NetTool.UintToIPBE((uint) i.GetLeafVar("30"));
                    // info.Port = (int) i.GetLeafVar("38");
                    // info.UploadId = (uint) i.GetLeafVar("48");
                    info.UseCached = true;

                    // Cached info
                    var imginfo = i.GetLeaf<ProtoTreeRoot>("32");
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
                // upload the image
                else
                {
                    info.Ip = (uint) i.GetLeafVar("38");
                    info.Host = NetTool.UintToIPBE(info.Ip);
                    info.Port = (int) i.GetLeafVar("40");
                    info.UploadTicket = i.GetLeafBytes("4A");
                    info.UseCached = false;
                }

                info.UploadId = i.GetLeafString("52");
                uploadInfo.Add(info);
            }

            // Construct event
            output = PicUpEvent.Result(0, uploadInfo);
            return true;
        }
    }

    protected override bool Build(int sequence, PicUpEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        if (input.Mode != UpMode.OffUp) return false;
        output.PutProtoNode(new OffPicUpRequest(appInfo, input.DestUin, input.SelfUin, input.UploadImages));
        return true;
    }
}

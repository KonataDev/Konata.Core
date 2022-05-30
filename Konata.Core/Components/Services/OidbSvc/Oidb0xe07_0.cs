using System.Collections.Generic;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Utils.Protobuf.ProtoModel;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(ImageOcrEvent))]
[Service("OidbSvc.0xe07_0", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0xe07_0 : BaseService<ImageOcrEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ImageOcrEvent output)
    {
        var tree = ProtoTreeRoot.Deserialize(input.Payload.GetBytes(), true);
        if (!tree.TryPathTo<ProtoLengthDelimited>("22.12", out var succ))
        {
            // Any error
            output = ImageOcrEvent.Result(-1, null);
            return true;
        }

        // if no result or failed
        if (succ.ToString() != "succ")
        {
            output = ImageOcrEvent.Result(0, null);
            return true;
        }

        // OCR Success
        if (tree.TryPathTo<ProtoTreeRoot>("22.52", out var detection))
        {
            // Add detections to result
            var result = new List<ImageOcrResult>();
            foreach (var item in detection.GetLeaves<ProtoTreeRoot>("0A"))
            {
                var text = item.GetLeafString("0A");
                var confidence = (int) item.GetLeafVar("10");
                result.Add(new ImageOcrResult(text, confidence));
            }

            output = ImageOcrEvent.Result(0, result);
            return true;
        }

        output = null;
        return false;
    }

    protected override bool Build(int sequence, ImageOcrEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0xe07_0(input.ImageUrl, input.ImageLength, input.ImageWidth, input.ImageHeight, input.ImageMd5);
        return true;
    }
}

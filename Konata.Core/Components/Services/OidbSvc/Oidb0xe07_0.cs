using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;

// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupSpecialTitleEvent))]
[Service("OidbSvc.0xe07_0", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0xe07_0 : BaseService<ImageOcrEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out ImageOcrEvent output)
    {
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

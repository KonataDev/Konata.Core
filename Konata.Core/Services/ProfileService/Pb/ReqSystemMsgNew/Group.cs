using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;
using Konata.Core.Common;

namespace Konata.Core.Services.ProfileService;

[Service("ProfileService.Pb.ReqSystemMsgNew.Group", "")]
internal class Group : BaseService<ProtocolEvent>
{
    protected override bool Parse(SSOFrame input,
         BotKeyStore keystore, out ProtocolEvent output)
    {
        output = null;
        return false;
    }

    protected override bool Build(Sequence sequence, ProtocolEvent input,
         BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = 0;
        return false;
    }
}


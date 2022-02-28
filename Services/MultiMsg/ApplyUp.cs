using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;

namespace Konata.Core.Services.MultiMsg;

internal class ApplyUp : BaseService<MessageApplyUpEvent>
{
    protected override bool Parse(SSOFrame input, BotKeyStore keystore,
        out MessageApplyUpEvent output)
    {
        return base.Parse(input, keystore, out output);
    }

    protected override bool Build(Sequence sequence, MessageApplyUpEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        newSequence = 0;
        output = null;
        return false;

    }
}

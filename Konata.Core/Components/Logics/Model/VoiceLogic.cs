using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;

// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(SharpSvrEvent))]
[BusinessLogic("Voice logic", "Friend voice client.")]
internal class VoiceLogic : BaseLogic
{
    internal VoiceLogic(BusinessComponent context) : base(context)
    {
    }

    public override Task Incoming(ProtocolEvent e)
    {
        if (e is not SharpSvrEvent sharp)
            return Task.CompletedTask;

        switch (sharp.Status)
        {
            // Ack
            case SharpSvrEvent.CallStatus.Ack:
                SharpSvrAck(Context, sharp);
                break;

            case SharpSvrEvent.CallStatus.AckMsf:
                SharpSvrAckMsf(Context, sharp);
                break;
            
            // Calle
            case SharpSvrEvent.CallStatus.CallIn:
                break;

            // unknown 5
            case SharpSvrEvent.CallStatus.Unknown5:
                break;
        }

        return base.Incoming(e);
    }

    public async Task<bool> CallFriend(uint friendUin)
    {
        var result = await CallFriend(Context, friendUin);
        return true;
    }

    #region Stub methods

    private static Task<SharpSvrEvent> CallFriend(BusinessComponent context, uint friendUin)
        => context.SendPacket<SharpSvrEvent>(SharpSvrEvent.CallOut(context.Bot.Uin, friendUin));

    private static void SharpSvrAck(BusinessComponent context, SharpSvrEvent e)
        => context.PostEvent<PacketComponent>(SharpSvrEvent.Ack(context.Bot.Uin, e));

    private static void SharpSvrAckMsf(BusinessComponent context, SharpSvrEvent e)
        => context.PostEvent<PacketComponent>(SharpSvrEvent.AckMsf(e.AckPayload));

    #endregion
}

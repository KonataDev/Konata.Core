﻿using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Attributes;
using Konata.Core.Events.Model;
using Konata.Core.Components.Model;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local

namespace Konata.Core.Logics.Model;

[EventSubscribe(typeof(PushConfigEvent))]
[EventSubscribe(typeof(OnlineReqPushEvent))]
[BusinessLogic("PushEvent Logic", "Forward push events to userend.")]
public class PushEventLogic : BaseLogic
{
    private const string TAG = "PushEvent Logic";

    internal PushEventLogic(BusinessComponent context)
        : base(context)
    {
    }

    public override void Incoming(ProtocolEvent e)
    {
        // TODO:
        // Confirm the push events with server

        switch (e)
        {
            // Handle push config
            case PushConfigEvent push:
                OnPushConfig(push);
                break;

            // Handle online push
            case OnlineReqPushEvent reqpush:
                OnOnlineReqPush(reqpush);
                break;

            // Just forward messages to userend
            default:
                Context.PostEventToEntity(e);
                break;
        }
    }

    /// <summary>
    /// Push config
    /// </summary>
    /// <param name="e"></param>
    private void OnPushConfig(PushConfigEvent e)
    {
        // Update the config
        ConfigComponent.HighwayConfig = new()
        {
            Server = e.HighwayHost,
            Ticket = e.HighwayTicket
        };
        Context.LogI(TAG, $"Highway server has changed {e.HighwayHost}");
    }

    /// <summary>
    /// Online push
    /// </summary>
    /// <param name="e"></param>
    private async void OnOnlineReqPush(OnlineReqPushEvent e)
    {
        // Post inner event
        if (e.InnerEvent != null)
            Context.PostEventToEntity(e.InnerEvent);

        // Confirm push
        await ConfrimReqPushEvent(Context, e);
    }

    #region Stub methods

    private static Task<OnlineRespPushEvent> ConfrimReqPushEvent(BusinessComponent context, OnlineReqPushEvent original)
        => context.SendPacket<OnlineRespPushEvent>(OnlineRespPushEvent.Create(context.Bot.Uin, original));

    #endregion
}

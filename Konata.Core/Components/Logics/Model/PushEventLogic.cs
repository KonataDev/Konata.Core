using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Events.Model;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace Konata.Core.Components.Logics.Model;

[EventSubscribe(typeof(PushConfigEvent))]
[EventSubscribe(typeof(PushNotifyEvent))]
[EventSubscribe(typeof(OnlineReqPushEvent))]
[EventSubscribe(typeof(PushTransMsgEvent))]

// Friend Messages
[EventSubscribe(typeof(FriendPokeEvent))]
[EventSubscribe(typeof(FriendTypingEvent))]
[EventSubscribe(typeof(FriendMessageRecallEvent))]

// Group Events
[EventSubscribe(typeof(GroupPokeEvent))]
[EventSubscribe(typeof(GroupMuteMemberEvent))]
[EventSubscribe(typeof(GroupMessageRecallEvent))]
[EventSubscribe(typeof(GroupKickMemberEvent))]
[EventSubscribe(typeof(GroupPromoteAdminEvent))]
[EventSubscribe(typeof(GroupMessageEvent))]
[BusinessLogic("PushEvent Logic", "Forward push events to userend.")]
internal class PushEventLogic : BaseLogic
{
    private const string TAG = "PushEvent Logic";
    private bool _tempFilter;

    internal PushEventLogic(BusinessComponent context)
        : base(context)
    {
        _tempFilter = false;
    }

    public override async Task Incoming(ProtocolEvent e)
    {
        switch (e)
        {
            // Handle push config
            case PushConfigEvent push:
                OnPushConfig(push);
                break;

            // Handle online push
            case OnlineReqPushEvent reqPush:
                await OnOnlineReqPush(reqPush);
                break;

            // Handle online push trans
            case PushTransMsgEvent transPush:
                await OnPushTransMsg(transPush);
                break;

            // Handle push notify event
            case PushNotifyEvent notifyPush:
                await OnPushNotify(notifyPush);
                break;

            // Just forward messages to user end
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
        Context.Bot.HighwayServer = new()
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
    private Task OnOnlineReqPush(OnlineReqPushEvent e)
    {
        var args = OnlineRespPushEvent.Create(Context.Bot.Uin, e);
        return Context.SendPacket<OnlineRespPushEvent>(args);
    }

    /// <summary>
    /// Trans msg push
    /// </summary>
    /// <param name="e"></param>
    private Task OnPushTransMsg(PushTransMsgEvent e)
    {
        var args = OnlineRespPushEvent.Create(Context.Bot.Uin, e);
        return Context.SendPacket<OnlineRespPushEvent>(args);
    }

    private async Task OnPushNotify(PushNotifyEvent e)
    {
        switch (e.Type)
        {
            case NotifyType.NewMember:
            case NotifyType.GroupCreated:
            case NotifyType.GroupRequestAccepted:
            case NotifyType.FriendMessage:
            case NotifyType.FriendMessageSingle:
            case NotifyType.FriendPttMessage:
            case NotifyType.StrangerMessage:
            case NotifyType.FriendFileMessage:
                await OnPullNewMessage();
                break;

            // TODO:
            // Fixme Filter
            case NotifyType.GroupRequest525:
                _tempFilter = !_tempFilter;
                if (_tempFilter) goto case NotifyType.GroupRequest;
                break;

            case NotifyType.GroupRequest:
            case NotifyType.GroupInvitation:
                await OnReqSystemGroupMsg();
                break;

            case NotifyType.FriendRequest:
            case NotifyType.FriendIncreaseSingle:
                await OnReqSystemFriendMsg();
                break;

            default:
            case NotifyType.BlackListUpdate:
                break;
        }
    }

    internal async Task OnPullNewMessage()
    {
        var args = PbGetMessageEvent.Create(Context.Bot.SyncCookie);
        var result = await Context.SendPacket<PbGetMessageEvent>(args);
        {
            // Update sync cookie
            if (result.SyncCookie != null)
                Context.Bot.SyncCookie = result.SyncCookie;
        }
    }

    internal Task OnReqSystemGroupMsg()
        => Context.SendPacket<ReqSystemMsgGroupEvent>(ReqSystemMsgGroupEvent.Create());
    
    internal Task OnReqSystemFriendMsg()
        => Context.SendPacket<ReqSystemMsgFriendEvent>(ReqSystemMsgFriendEvent.Create());
    
}

namespace Konata.Core.Events.Model;

internal class PushNotifyEvent : ProtocolEvent
{
    /// <summary>
    /// <b>[Out]</b> <br/>
    /// Notify type
    /// </summary>
    public NotifyType Type { get; }

    private PushNotifyEvent(NotifyType type) : base(0)
    {
        Type = type;
    }

    /// <summary>
    /// Construct event push
    /// </summary>
    /// <returns></returns>
    internal static PushNotifyEvent Push(NotifyType type)
        => new(type);

}

internal enum NotifyType
{
    NewMember = 33,//群员入群

    GroupCreated = 38, //建群

    StrangerMessage = 141, //陌生人

    FriendMessage = 166, //好友

    FriendMessageSingle = 167, //单向好友

    FriendPttMessage = 208,

    FriendFileMessage = 529,

    GroupInvitation = 87, //群邀请

    GroupRequest = 84, //加群申请

    GroupRequest525 = 525, // 加群申请(来自群员的邀请)

    GroupRequestAccepted = 85,//群申请被同意

    FriendRequest = 187,

    FriendIncreaseSingle = 191,

    BlackListUpdate = 528
}

using System;
using System.Text;
using System.Threading.Tasks;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Entity;
using Konata.Core.Message;
using System.Collections.Generic;

namespace Konata.Core.Component
{
    [Component("BusinessComponent", "Konata Business Component")]
    public class BusinessComponent : BaseComponent
    {
        public string TAG = "BusinessComponent";

        private OnlineStatusEvent.Type _onlineType;
        private TaskCompletionSource<WtLoginEvent> _userOperation;

        public BusinessComponent()
        {
            _onlineType = OnlineStatusEvent.Type.Offline;
        }

        public async Task<bool> Login()
        {
            if (_onlineType == OnlineStatusEvent.Type.Offline)
            {
                var socketComp = GetComponent<SocketComponent>();
                if (!await socketComp.Connect(true))
                {
                    return false;
                }

                var wtStatus = await WtLogin();
                {
                    while (true)
                    {
                        switch (wtStatus.EventType)
                        {
                            case WtLoginEvent.Type.OK:
                                if ((await SetClientOnineType(OnlineStatusEvent.Type.Online)).
                                    EventType == OnlineStatusEvent.Type.Online)
                                {
                                    return true;
                                }
                                else
                                {
                                    await socketComp.DisConnect("Wtlogin failed.");
                                    return false;
                                }

                            case WtLoginEvent.Type.CheckSMS:
                            case WtLoginEvent.Type.CheckSlider:
                                PostEventToEntity(wtStatus);
                                wtStatus = await WtCheckUserOperation();
                                break;

                            case WtLoginEvent.Type.RefreshSMS:
                                wtStatus = await WtRefreshSMSCode();
                                break;

                            case WtLoginEvent.Type.CheckDevLock:
                            //wtStatus = await WtValidateDeviceLock();
                            //break;

                            case WtLoginEvent.Type.LoginDenied:
                            case WtLoginEvent.Type.InvalidSmsCode:
                            case WtLoginEvent.Type.InvalidLoginEnvironment:
                            case WtLoginEvent.Type.InvalidUinOrPassword:
                                PostEventToEntity(wtStatus);
                                return false;

                            default:
                            case WtLoginEvent.Type.NotImplemented:
                                LogW(TAG, "Login fail. Unsupported wtlogin event type received.");
                                return false;
                        }
                    }
                }

                LogW(TAG, "You goes here? What the happend?");
                return false;
            }

            LogW(TAG, "Calling Login method again while online.");
            return false;
        }

        public void SubmitSMSCode(string code)
            => _userOperation.SetResult(new WtLoginEvent
            { EventType = WtLoginEvent.Type.CheckSMS, CaptchaResult = code });

        public void SubmitSliderTicket(string ticket)
            => _userOperation.SetResult(new WtLoginEvent
            { EventType = WtLoginEvent.Type.CheckSlider, CaptchaResult = ticket });

        internal async Task<WtLoginEvent> WtLogin()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.Tgtgt });

        internal async Task<WtLoginEvent> WtRefreshSMSCode()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.RefreshSMS });

        internal async Task<WtLoginEvent> WtValidateDeviceLock()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.CheckDevLock });

        internal async Task<WtLoginEvent> WtCheckUserOperation()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (await WaitForUserOperation());

        internal async Task<OnlineStatusEvent> SetClientOnineType(OnlineStatusEvent.Type onlineType)
            => (OnlineStatusEvent)await PostEvent<PacketComponent>
            (new OnlineStatusEvent { EventType = onlineType });

        public async Task<GroupKickMemberEvent> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
            => (GroupKickMemberEvent)await PostEvent<PacketComponent>
                (new GroupKickMemberEvent
                {
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = preventRequest
                });

        public async Task<GroupPromoteAdminEvent> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
            => (GroupPromoteAdminEvent)await PostEvent<PacketComponent>
                (new GroupPromoteAdminEvent
                {
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = toggleAdmin
                });

        internal async void ConfirmReadGroupMessage(GroupMessageEvent groupMessage)
            => await PostEvent<PacketComponent>
                (new GroupMessageReadEvent
                {
                    GroupUin = groupMessage.GroupUin,
                    RequestId = groupMessage.MessageId,
                    SessionSequence = groupMessage.SessionSequence,
                });

        internal async void PrivateMessagePulldown()
            => await PostEvent<PacketComponent>(new PrivateMessagePullEvent
            {
                SyncCookie = GetComponent<ConfigComponent>().SignInfo.SyncCookie
            });

        internal void ConfirmPrivateMessage(PrivateMessageEvent privateMessage)
        {
            GetComponent<ConfigComponent>().SignInfo.SyncCookie
                = privateMessage.SyncCookie;
        }

        private async Task<WtLoginEvent> WaitForUserOperation()
        {
            _userOperation = new TaskCompletionSource<WtLoginEvent>();
            return await _userOperation.Task;
        }

        public async Task<GroupMessageEvent> SendGroupMessage(uint groupUin, List<MessageChain> message)
          => (GroupMessageEvent)await PostEvent<PacketComponent>
            (new GroupMessageEvent
            {
                GroupUin = groupUin,
                Message = message
            });

        internal override void EventHandler(KonataTask task)
        {
            switch (task.EventPayload)
            {
                // Receive online status from server
                case OnlineStatusEvent onlineStatusEvent:
                    _onlineType = onlineStatusEvent.EventType;
                    break;

                // Confirm with server about we have read group message
                case GroupMessageEvent groupMessageEvent:
                    ConfirmReadGroupMessage(groupMessageEvent);
                    goto default;

                // Pull the private message when notified
                case PrivateMessageNotifyEvent _:
                    PrivateMessagePulldown();
                    break;

                // Confirm with server about we have read private message
                case PrivateMessageEvent privateMessageEvent:
                    ConfirmPrivateMessage(privateMessageEvent);
                    goto default;

                // Pass messages to upstream
                default:
                    PostEventToEntity(task.EventPayload);
                    break;
            }
        }
    }
}

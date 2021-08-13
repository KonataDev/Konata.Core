using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Components.Model;

// ReSharper disable RedundantCaseLabel
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(WtLoginEvent))]
    [EventSubscribe(typeof(OnlineStatusEvent))]
    [BusinessLogic("Wtlogin Exchange Logic", "Responsible for the online tasks.")]
    public class WtExchangeLogic : BaseLogic
    {
        private const string TAG = "WtXchg Logic";
        private const string ScheduleCheckConn = "Logic.WtXchg.CheckOnline";
        private const string ScheduleHeartBeat = "Logic.WtXchg.HeartBeat";

        private OnlineStatusEvent.Type _onlineType;
        private TaskCompletionSource<WtLoginEvent> _userOperation;

        public OnlineStatusEvent.Type OnlineType
            => _onlineType;

        internal WtExchangeLogic(BusinessComponent context)
            : base(context)
        {
            _onlineType = OnlineStatusEvent.Type.Offline;
        }

        public override void Incoming(ProtocolEvent e)
        {
            // Receive online status from server
            if (e is OnlineStatusEvent status)
            {
                _onlineType = status.EventType;
            }
        }

        /// <summary>
        /// WtExchange login
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Login()
        {
            // Check online type
            if (_onlineType != OnlineStatusEvent.Type.Offline)
            {
                Context.LogW(TAG, "Calling Login method again while online.");
                return false;
            }

            // Connect to the server
            if (!await SocketComponent.Connect(true))
            {
                return false;
            }

            // Login
            var wtStatus = await WtLogin();
            {
                while (true)
                {
                    switch (wtStatus.EventType)
                    {
                        case WtLoginEvent.Type.OK:

                            // Set online
                            var online = await SetClientOnineType(OnlineStatusEvent.Type.Online);
                        {
                            _onlineType = online.EventType;

                            // Bot online
                            if (online.EventType == OnlineStatusEvent.Type.Online)
                            {
                                // Online
                                Context.PostEventToEntity(online);

                                // Register schedules
                                Context.ScheduleComponent.Interval(ScheduleHeartBeat, 600, OnSendHeartBeat);
                                Context.ScheduleComponent.Interval(ScheduleCheckConn, 60, OnCheckConnection);

                                return true;
                            }

                            SocketComponent.DisConnect("Wtlogin failed.");
                            return false;
                        }

                        case WtLoginEvent.Type.CheckSms:
                        case WtLoginEvent.Type.CheckSlider:
                            Context.PostEventToEntity(wtStatus);
                            wtStatus = await WtCheckUserOperation();
                            break;

                        case WtLoginEvent.Type.RefreshSMS:
                            wtStatus = await WtRefreshSmsCode();
                            break;

                        case WtLoginEvent.Type.CheckDevLock:
                        //wtStatus = await WtValidateDeviceLock();
                        //break;

                        case WtLoginEvent.Type.LoginDenied:
                        case WtLoginEvent.Type.InvalidSmsCode:
                        case WtLoginEvent.Type.InvalidLoginEnvironment:
                        case WtLoginEvent.Type.InvalidUinOrPassword:
                            Context.PostEventToEntity(wtStatus);
                            Context.SocketComponent.DisConnect("Wtlogin failed.");
                            return false;

                        default:
                        case WtLoginEvent.Type.Unknown:
                        case WtLoginEvent.Type.NotImplemented:
                            Context.SocketComponent.DisConnect("Wtlogin failed.");
                            Context.LogW(TAG, "Login fail. Unsupported wtlogin event type received.");
                            return false;
                    }
                }
            }
        }

        public Task<bool> Logout()
            => Task.FromResult(SocketComponent.DisConnect("user logout"));

        private Task<WtLoginEvent> WtLogin()
            => Context.PostEvent<PacketComponent, WtLoginEvent>(new WtLoginEvent {EventType = WtLoginEvent.Type.Tgtgt});

        private Task<WtLoginEvent> WtRefreshSmsCode()
            => Context.PostEvent<PacketComponent, WtLoginEvent>(new WtLoginEvent
                {EventType = WtLoginEvent.Type.RefreshSMS});

        private Task<WtLoginEvent> WtValidateDeviceLock()
            => Context.PostEvent<PacketComponent, WtLoginEvent>(new WtLoginEvent
                {EventType = WtLoginEvent.Type.CheckDevLock});

        private Task<WtLoginEvent> WtCheckUserOperation()
            => Context.PostEvent<PacketComponent, WtLoginEvent>(WaitForUserOperation().Result);

        private Task<OnlineStatusEvent> SetClientOnineType(OnlineStatusEvent.Type onlineType)
            => Context.PostEvent<PacketComponent, OnlineStatusEvent>(new OnlineStatusEvent {EventType = onlineType});

        public void SubmitSmsCode(string code)
            => _userOperation.SetResult(new WtLoginEvent
                {EventType = WtLoginEvent.Type.CheckSms, CaptchaResult = code});

        public void SubmitSliderTicket(string ticket)
            => _userOperation.SetResult(new WtLoginEvent
                {EventType = WtLoginEvent.Type.CheckSlider, CaptchaResult = ticket});

        private async Task<WtLoginEvent> WaitForUserOperation()
        {
            _userOperation = new TaskCompletionSource<WtLoginEvent>();
            return await _userOperation.Task;
        }

        public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
        {
            if (_onlineType == status)
            {
                return Task.FromResult(true);
            }

            switch (_onlineType)
            {
                // Login
                case OnlineStatusEvent.Type.Offline:
                    return Login();

                // Not supported yet
                case OnlineStatusEvent.Type.Online:
                case OnlineStatusEvent.Type.Leave:
                case OnlineStatusEvent.Type.Busy:
                case OnlineStatusEvent.Type.Hidden:
                case OnlineStatusEvent.Type.QMe:
                case OnlineStatusEvent.Type.DoNotDistrub:
                    return Task.FromResult(false);
            }

            return Task.FromResult(false);
        }

        private void OnCheckConnection()
        {
            // TODO:
            // Check connection
        }

        private void OnSendHeartBeat()
        {
            // TODO:
            // Check heartbeat
        }
    }
}

using System;
using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Components.Model;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable RedundantCaseLabel
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(WtLoginEvent))]
    [EventSubscribe(typeof(OnlineStatusEvent))]
    [BusinessLogic("Wtlogin Exchange Logic", "Responsible for the online tasks.")]
    public class WtExchangeLogic : BaseLogic
    {
        private const string TAG = "WtXchg Logic";
        private const string ScheduleKeepOnline = "Logic.WtXchg.KeepOnline";
        private const string ScheduleCheckConnection = "Logic.WtXchg.CheckConnection";

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
                OnStatusChanged(status);
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

            try
            {
                // Login
                var wtStatus = await WtLogin(Context);
                {
                    while (true)
                    {
                        switch (wtStatus.EventType)
                        {
                            case WtLoginEvent.Type.OK:

                                // Set online
                                var online = await SetClientOnineType(Context, OnlineStatusEvent.Type.Online);

                                // Update online status
                                if (online.EventType == OnlineStatusEvent.Type.Online)
                                {
                                    // Bot online
                                    Context.PostEventToEntity(online);
                                    await Context.PostEvent<BusinessComponent>(online);


                                    return true;
                                }

                                // Oops...
                                SocketComponent.Disconnect("Wtlogin failed.");
                                return false;

                            case WtLoginEvent.Type.CheckSms:
                            case WtLoginEvent.Type.CheckSlider:
                                Context.PostEventToEntity(wtStatus);
                                wtStatus = await WtCheckUserOperation(Context, await WaitForUserOperation());
                                break;

                            case WtLoginEvent.Type.RefreshSMS:
                                wtStatus = await WtRefreshSmsCode(Context);
                                break;

                            case WtLoginEvent.Type.CheckDevLock:
                            //wtStatus = await WtValidateDeviceLock();
                            //break;

                            case WtLoginEvent.Type.LoginDenied:
                            case WtLoginEvent.Type.InvalidSmsCode:
                            case WtLoginEvent.Type.InvalidLoginEnvironment:
                            case WtLoginEvent.Type.InvalidUinOrPassword:
                                Context.PostEventToEntity(wtStatus);
                                Context.SocketComponent.Disconnect("Wtlogin failed.");
                                return false;

                            default:
                            case WtLoginEvent.Type.Unknown:
                            case WtLoginEvent.Type.NotImplemented:
                                Context.SocketComponent.Disconnect("Wtlogin failed.");
                                Context.LogW(TAG, "Login fail. Unsupported wtlogin event type received.");
                                return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SocketComponent.Disconnect("Timed out");

                Context.LogE(TAG, "Request timed out.");
                Context.LogE(TAG, e);

                return false;
            }
        }

        public Task<bool> Logout()
            => Task.FromResult(SocketComponent.Disconnect("user logout"));

        public void SubmitSmsCode(string code)
            => _userOperation.SetResult(WtLoginEvent.CreateSubmitSmsCode(code));

        public void SubmitSliderTicket(string ticket)
            => _userOperation.SetResult(WtLoginEvent.CreateSubmitTicket(ticket));

        private Task<WtLoginEvent> WaitForUserOperation()
        {
            _userOperation = new TaskCompletionSource<WtLoginEvent>();
            return _userOperation.Task;
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
                case OnlineStatusEvent.Type.Online:
                    return Login();

                // Not supported yet
                case OnlineStatusEvent.Type.Offline:
                case OnlineStatusEvent.Type.Leave:
                case OnlineStatusEvent.Type.Busy:
                case OnlineStatusEvent.Type.Hidden:
                case OnlineStatusEvent.Type.QMe:
                case OnlineStatusEvent.Type.DoNotDistrub:
                    return Task.FromResult(false);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Online status changed
        /// </summary>
        /// <param name="status"></param>
        private void OnStatusChanged(OnlineStatusEvent status)
        {
            _onlineType = status.EventType;

            switch (status.EventType)
            {
                // Bot come online
                case OnlineStatusEvent.Type.Online:
                    // Register schedules
                    Context.ScheduleComponent.Interval(ScheduleKeepOnline, 600 * 1000, OnKeepOnline);
                    Context.ScheduleComponent.Interval(ScheduleCheckConnection, 60 * 1000, OnCheckConnection);
                    break;

                // Bot offline
                case OnlineStatusEvent.Type.Offline:
                    // Cancel schedules
                    Context.ScheduleComponent.Cancel(ScheduleKeepOnline);
                    Context.ScheduleComponent.Cancel(ScheduleCheckConnection);

                    // Disconnect
                    Logout();
                    break;
            }
        }

        /// <summary>
        /// Check connection
        /// </summary>
        private async void OnCheckConnection()
        {
            // TODO:
            // Check connection
            Context.LogI(TAG, "OnCheckConnection");

            try
            {
                await CheckHeartbeat(Context);
            }
            catch (TimeoutException e)
            {
                Context.LogW(TAG, "The client was offline.");
                Context.LogE(TAG, e);

                // Check if reconnect
                if (ConfigComponent.GlobalConfig.ReConnectWhileLinkDown)
                {
                    // TODO: 
                    // Reconnect
                    Context.LogW(TAG, "TODO: Reconnect.");
                }

                // Go offline
                else
                {
                    _onlineType = OnlineStatusEvent.Type.Offline;
                    await Context.PostEvent<BusinessComponent>
                        (OnlineStatusEvent.Push(_onlineType, "Heart broken"));
                }
            }
        }

        /// <summary>
        /// Keep online
        /// </summary>
        private void OnKeepOnline()
        {
            // TODO:
            // Keep online
            Context.LogI(TAG, "OnKeepOnline");
        }

        #region Stub methods

        private static Task<WtLoginEvent> WtLogin(BusinessComponent context)
            => context.PostPacket<WtLoginEvent>(WtLoginEvent.CreateTgtgt());

        private static Task<WtLoginEvent> WtRefreshSmsCode(BusinessComponent context)
            => context.PostPacket<WtLoginEvent>(WtLoginEvent.CreateRefreshSms());

        private static Task<WtLoginEvent> WtValidateDeviceLock(BusinessComponent context)
            => context.PostPacket<WtLoginEvent>(WtLoginEvent.CreateCheckDevLock());

        private static Task<WtLoginEvent> WtCheckUserOperation(BusinessComponent context, WtLoginEvent userOperation)
            => context.PostPacket<WtLoginEvent>(userOperation);

        private static Task<OnlineStatusEvent> SetClientOnineType(BusinessComponent context, OnlineStatusEvent.Type onlineType)
            => context.PostPacket<OnlineStatusEvent>(OnlineStatusEvent.Create(onlineType));

        private static Task<CheckHeartbeatEvent> CheckHeartbeat(BusinessComponent context)
            => context.PostPacket<CheckHeartbeatEvent>(CheckHeartbeatEvent.Create());

        #endregion
    }
}

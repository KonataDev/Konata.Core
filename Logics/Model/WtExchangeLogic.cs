using System;
using System.Threading;
using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Components.Model;
using Konata.Core.Utils.IO;

// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable RedundantCaseLabel
// ReSharper disable ClassNeverInstantiated.Global

namespace Konata.Core.Logics.Model
{
    [EventSubscribe(typeof(ProtocolEvent))]
    [EventSubscribe(typeof(OnlineStatusEvent))]
    [BusinessLogic("Wtlogin Exchange Logic", "Responsible for the online tasks.")]
    public class WtExchangeLogic : BaseLogic
    {
        private const string TAG = "WtXchg Logic";
        private const string ScheduleKeepOnline = "Logic.WtXchg.KeepOnline";
        private const string ScheduleCheckConnection = "Logic.WtXchg.CheckConnection";

        private OnlineStatusEvent.Type _onlineType;
        private TaskCompletionSource<WtLoginEvent> _userOperation;
        private uint _heartbeatCounter;

        public OnlineStatusEvent.Type OnlineType
            => _onlineType;

        internal WtExchangeLogic(BusinessComponent context)
            : base(context)
        {
            _onlineType = OnlineStatusEvent.Type.Offline;
            _heartbeatCounter = 0;
        }

        public override void Incoming(ProtocolEvent e)
        {
            // Receive online status from server
            if (e is OnlineStatusEvent status)
            {
                OnStatusChanged(status);
            }

            _heartbeatCounter++;
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
            Context.LogI(TAG, "Connecting server...");
            if (!await SocketComponent.Connect(true))
            {
                return false;
            }

            try
            {
                WtLoginEvent wtStatus;

                // Can I fast login?
                if (ConfigComponent.KeyStore.Session.D2Key.Length != 0
                    && ConfigComponent.KeyStore.Session.D2Token.Length != 0)
                {
                    // Okay, We can try it
                    Context.LogI(TAG, "Do WtXchg");

                    try
                    {
                        wtStatus = await WtXchg(Context);

                        // Success
                        if (wtStatus.EventType == WtLoginEvent.Type.OK)
                        {
                            goto GirlBlessingQwQ;
                        }

                    }
                    catch (Exception e)
                    {
                        // Do nothing
                    }
                    
                    Context.LogI(TAG, "Fast login failed.");
                }

                // Wtlogin
                Context.LogI(TAG, "Do Wtlogin");
                wtStatus = await WtLogin(Context);

            GirlBlessingQwQ:
                while (true)
                {
                    Context.LogI(TAG, $"Status => {wtStatus.EventType}");
                    switch (wtStatus.EventType)
                    {
                        case WtLoginEvent.Type.OK:

                            // Dump keys
                            Context.LogV(TAG, "Keystore Dump");
                            Context.LogV(TAG, $"  D2Key    {ByteConverter.Hex(ConfigComponent.KeyStore.Session.D2Key)}");
                            Context.LogV(TAG, $"  D2Token  {ByteConverter.Hex(ConfigComponent.KeyStore.Session.D2Token)}");
                            Context.LogV(TAG, $"  Tgtgt    {ByteConverter.Hex(ConfigComponent.KeyStore.Session.TgtKey)}");
                            Context.LogV(TAG, $"  TgtToken {ByteConverter.Hex(ConfigComponent.KeyStore.Session.TgtToken)}");

                            // Set online
                            Context.LogI(TAG, "Registering client");
                            var online = await SetClientOnineType(Context, OnlineStatusEvent.Type.Online);

                            // Update online status
                            if (online.EventType == OnlineStatusEvent.Type.Online)
                            {
                                // Bot online
                                Context.PostEventToEntity(online);
                                await Context.PostEvent<BusinessComponent>(online);

                                Context.LogI(TAG, "Bot online.");
                                return true;
                            }

                            // Oops...
                            SocketComponent.Disconnect("Wtlogin failed.");
                            return false;

                        case WtLoginEvent.Type.CheckSms:
                        case WtLoginEvent.Type.CheckSlider:

                            // Check handler
                            if (!Context.Bot.HandlerRegistered<CaptchaEvent>())
                            {
                                Context.SocketComponent.Disconnect("Need handler.");
                                Context.LogW(TAG, "No captcha event handler registered, " +
                                                  "Please note, Konata cannot process captcha automatically.");
                                return false;
                            }

                            // Wait for user operation
                            Context.PostEventToEntity(CaptchaEvent.Create(wtStatus));
                            wtStatus = await WtCheckUserOperation(Context, await WaitForUserOperation());
                            break;

                        case WtLoginEvent.Type.RefreshSms:
                            wtStatus = await WtRefreshSmsCode(Context);
                            break;

                        case WtLoginEvent.Type.RefreshSmsFailed:
                            Context.LogW(TAG, "Send sms failed, Konata " +
                                              "will resend the sms after 60 sec.");
                            Thread.Sleep(60 * 1000);
                            wtStatus = await WtRefreshSmsCode(Context);
                            break;

                        case WtLoginEvent.Type.VerifyDeviceLock:
                            wtStatus = await WtVerifyDeviceLock(Context);
                            break;

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
                            Context.LogE(TAG, "Login fail. Unsupported wtlogin event type received.");
                            return false;
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
                    Context.ScheduleComponent.Interval(ScheduleCheckConnection, 600 * 1000, OnCheckConnection);
                    break;

                // Bot offline
                case OnlineStatusEvent.Type.Offline:
                    // Cancel schedules
                    Context.ScheduleComponent.Cancel(ScheduleKeepOnline);
                    Context.ScheduleComponent.Cancel(ScheduleCheckConnection);
                    break;
            }
        }

        /// <summary>
        /// Check connection
        /// </summary>
        private async void OnCheckConnection()
        {
            // Check connection
            Context.LogI(TAG, "OnCheckConnection");

            // Client alive
            // So reset the counter
            if (_heartbeatCounter > 0)
            {
                _heartbeatCounter = 0;
                return;
            }

            try
            {
                // Check heartbeat
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
                    SocketComponent.Disconnect("Heart broken.");
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

        private static Task<WtLoginEvent> WtXchg(BusinessComponent context)
            => context.PostPacket<WtLoginEvent>(WtLoginEvent.CreateXchg());

        private static Task<WtLoginEvent> WtRefreshSmsCode(BusinessComponent context)
            => context.PostPacket<WtLoginEvent>(WtLoginEvent.CreateRefreshSms());

        private static Task<WtLoginEvent> WtVerifyDeviceLock(BusinessComponent context)
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

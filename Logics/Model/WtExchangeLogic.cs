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
        private const string SchedulePullMessage = "Logic.WtXchg.PullMessage";
        private const string ScheduleCheckConnection = "Logic.WtXchg.CheckConnection";

        private OnlineStatusEvent.Type _onlineType;
        private TaskCompletionSource<WtLoginEvent> _userOperation;
        private uint _heartbeatCounter;
        private bool _useFastLogin;

        public OnlineStatusEvent.Type OnlineType
            => _onlineType;

        internal WtExchangeLogic(BusinessComponent context)
            : base(context)
        {
            _onlineType = OnlineStatusEvent.Type.Offline;
            _heartbeatCounter = 0;
            _useFastLogin = false;
        }

        public override void Incoming(ProtocolEvent e)
        {
            // Receive online status from server
            if (e is OnlineStatusEvent status)
            {
                // Online status changed
                _onlineType = status.EventType;
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
                        _useFastLogin = true;
                        wtStatus = await WtXchg(Context);

                        // Success
                        if (wtStatus.EventType == WtLoginEvent.Type.OK)
                        {
                            goto GirlBlessingQwQ;
                        }
                    }
                    catch
                    {
                        // Do nothing
                    }

                    Context.LogI(TAG, "Fast login failed.");
                }

                // Wtlogin
                Context.LogI(TAG, "Do Wtlogin");

                _useFastLogin = false;
                wtStatus = await WtLogin(Context);

            GirlBlessingQwQ:
                while (true)
                {
                    Context.LogI(TAG, $"Status => {wtStatus.EventType}");
                    switch (wtStatus.EventType)
                    {
                        case WtLoginEvent.Type.OK:
                            return await OnBotOnline();

                        case WtLoginEvent.Type.CheckSms:
                        case WtLoginEvent.Type.CheckSlider:

                            // Check handler
                            if (!Context.Bot.HandlerRegistered<CaptchaEvent>())
                            {
                                await Context.SocketComponent.Disconnect("Need handler.");
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
                        case WtLoginEvent.Type.HighRiskEnvironment:
                        case WtLoginEvent.Type.InvalidUinOrPassword:
                            await Context.SocketComponent.Disconnect("Wtlogin failed.");
                            return false;

                        default:
                        case WtLoginEvent.Type.Unknown:
                        case WtLoginEvent.Type.NotImplemented:
                            await Context.SocketComponent.Disconnect("Wtlogin failed.");
                            Context.LogE(TAG, "Login fail. Unsupported wtlogin event type received.");
                            return false;
                    }
                }
            }
            catch (Exception e)
            {
                await SocketComponent.Disconnect(e.Message);
                Context.LogE(TAG, e);

                return false;
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public Task<bool> Logout()
        {
            // Cancel schedules
            ScheduleComponent.Cancel(SchedulePullMessage);
            ScheduleComponent.Cancel(ScheduleCheckConnection);

            // Push offline
            Context.PostEvent<BusinessComponent>
               (OnlineStatusEvent.Push(OnlineStatusEvent.Type.Offline, "user logout"));

            return SocketComponent.Disconnect("user logout");
        }

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
        /// On bot online
        /// </summary>
        /// <returns></returns>
        private async Task<bool> OnBotOnline()
        {
            // Dump keys
            Context.LogV(TAG, "Keystore Dump");
            Context.LogV(TAG, $"  D2Key    {ByteConverter.Hex(ConfigComponent.KeyStore.Session.D2Key)}");
            Context.LogV(TAG, $"  D2Token  {ByteConverter.Hex(ConfigComponent.KeyStore.Session.D2Token)}");
            Context.LogV(TAG, $"  Tgtgt    {ByteConverter.Hex(ConfigComponent.KeyStore.Session.TgtKey)}");
            Context.LogV(TAG, $"  TgtToken {ByteConverter.Hex(ConfigComponent.KeyStore.Session.TgtToken)}");

            // Set online
            Context.LogI(TAG, "Registering client");

            try
            {
                var online = await SetClientOnineType(Context, OnlineStatusEvent.Type.Online);

                // Update online status
                if (online.EventType == OnlineStatusEvent.Type.Online)
                {
                    // Bot online
                    Context.PostEventToEntity(online);
                    await Context.PostEvent<BusinessComponent>(online);

                    // Register schedules
                    ScheduleComponent.Interval(SchedulePullMessage, 500 * 1000, OnPullMessage);
                    ScheduleComponent.Interval(ScheduleCheckConnection, 600 * 1000, OnCheckConnection);

                    Context.LogI(TAG, "Bot online.");
                    return true;
                }
            }

            catch (Exception e)
            {
                Context.LogE(TAG, e);
                return false;
            }

            if (_useFastLogin)
            {
                Context.LogI(TAG, "Fast login failed, " +
                                  "Relogin with password.");

                // Clear the old key
                ConfigComponent.KeyStore.Session.D2Key = Array.Empty<byte>();
                ConfigComponent.KeyStore.Session.D2Token = Array.Empty<byte>();

                // Do login again
                return await Login();
            }

            // Oops...
            await SocketComponent.Disconnect("Online failed.");
            return false;
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
                if (ConfigComponent.GlobalConfig.TryReconnect)
                {
                    Context.LogW(TAG, "Reconnect.");

                    // Reconnect
                    if (await SocketComponent.Reconnect())
                    {
                        // Bot reonline
                        if (await OnBotOnline())
                        {
                            Context.LogI(TAG, "Network reset.");
                            return;
                        }

                        Context.LogW(TAG, "Reconnect failed! " +
                                          "Might need to relogin?");
                    }
                }

                // Go offline
                await SocketComponent.Disconnect("Heart broken.");

                // Cancel schedules
                ScheduleComponent.Cancel(SchedulePullMessage);
                ScheduleComponent.Cancel(ScheduleCheckConnection);
            }
        }

        /// <summary>
        /// Pull new message
        /// </summary>
        private async void OnPullMessage()
        {
            // Pull message
            Context.LogI(TAG, "OnPullMessage");

            try
            {
                // Get new message
                await PullMessage(Context);
            }
            catch (TimeoutException e)
            {
                Context.LogW(TAG, "Connection lost? " +
                                  "Let me check the connection.");

                // Check the connection
                _heartbeatCounter = 0;
                ScheduleComponent.Trigger(ScheduleCheckConnection);
            }
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

        private static Task<PullMessageEvent> PullMessage(BusinessComponent context)
            => context.PostPacket<PullMessageEvent>(PullMessageEvent.Create(context.ConfigComponent.SyncCookie));

        #endregion
    }
}

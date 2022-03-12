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
        private const string ScheduleReLogin = "Logic.WtXchg.ReLogin";

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
            _useFastLogin = false;
            _heartbeatCounter = 0;
        }

        public override Task Incoming(ProtocolEvent e)
        {
            // Receive online status from server
            if (e is OnlineStatusEvent status)
            {
                // Online status changed
                _onlineType = status.EventType;
            }

            _heartbeatCounter++;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Bot login
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
        /// Disconnect the socket and logout
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

        /// <summary>
        /// Submit Slider ticket while login
        /// </summary>
        /// <param name="ticket"><b>[In]</b> Slider ticket</param>
        public bool SubmitSliderTicket(string ticket)
        {
            if (_userOperation == null) return false;
            if (_userOperation.Task.IsCompleted) return false;

            _userOperation.SetResult(WtLoginEvent.CreateSubmitTicket(ticket));
            return true;
        }

        /// <summary>
        /// Submit Sms code while login
        /// </summary>
        /// <param name="code"><b>[In]</b> Sms code</param>
        public bool SubmitSmsCode(string code)
        {
            if (_userOperation == null) return false;
            if (_userOperation.Task.IsCompleted) return false;

            _userOperation.SetResult(WtLoginEvent.CreateSubmitSmsCode(code));
            return true;
        }

        /// <summary>
        /// Wait for user operation
        /// </summary>
        /// <returns></returns>
        private Task<WtLoginEvent> WaitForUserOperation()
        {
            _userOperation = new TaskCompletionSource<WtLoginEvent>();
            return _userOperation.Task;
        }

        /// <summary>
        /// Set online status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
                    await Context.SendEvent<BusinessComponent>(online);

                    // Register schedules
                    ScheduleComponent.Interval(SchedulePullMessage, 180 * 1000, OnPullMessage);
                    ScheduleComponent.Interval(ScheduleCheckConnection, 60 * 1000, OnCheckConnection);

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

                // Disconnect
                await SocketComponent.Disconnect("Heart broken.");

                // Cancel schedules
                ScheduleComponent.Cancel(SchedulePullMessage);
                ScheduleComponent.Cancel(ScheduleCheckConnection);

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

                    // Relogin
                    ScheduleComponent.Interval(ScheduleReLogin, 10 * 1000, 6, OnReConnect);
                }
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
                var result = await PullMessage(Context);
                await Context.PushEvent.Incoming(result);
            }
            catch (TimeoutException)
            {
                Context.LogW(TAG, "Connection lost? " +
                                  "Let me check the connection.");

                // Check the connection
                _heartbeatCounter = 0;
                ScheduleComponent.Trigger(ScheduleCheckConnection);
            }
        }

        /// <summary>
        /// Reconnect
        /// </summary>
        private async void OnReConnect()
        {
            try
            {
                // Close socket
                if (SocketComponent.Connected)
                    await SocketComponent.Disconnect("Try Relogin");

                _onlineType = OnlineStatusEvent.Type.Offline;
                if (await Login()) ScheduleComponent.Cancel(ScheduleReLogin);
            }
            catch (TimeoutException e)
            {
                Context.LogW(TAG, "ReLogin failed?");
                Context.LogE(TAG, e);
            }
        }

        #region Stub methods

        private static Task<WtLoginEvent> WtLogin(BusinessComponent context)
            => context.SendPacket<WtLoginEvent>(WtLoginEvent.CreateTgtgt());

        private static Task<WtLoginEvent> WtXchg(BusinessComponent context)
            => context.SendPacket<WtLoginEvent>(WtLoginEvent.CreateXchg());

        private static Task<WtLoginEvent> WtRefreshSmsCode(BusinessComponent context)
            => context.SendPacket<WtLoginEvent>(WtLoginEvent.CreateRefreshSms());

        private static Task<WtLoginEvent> WtVerifyDeviceLock(BusinessComponent context)
            => context.SendPacket<WtLoginEvent>(WtLoginEvent.CreateCheckDevLock());

        private static Task<WtLoginEvent> WtCheckUserOperation(BusinessComponent context, WtLoginEvent userOperation)
            => context.SendPacket<WtLoginEvent>(userOperation);

        private static Task<OnlineStatusEvent> SetClientOnineType(BusinessComponent context, OnlineStatusEvent.Type onlineType)
            => context.SendPacket<OnlineStatusEvent>(OnlineStatusEvent.Create(onlineType));

        private static Task<CheckHeartbeatEvent> CheckHeartbeat(BusinessComponent context)
            => context.SendPacket<CheckHeartbeatEvent>(CheckHeartbeatEvent.Create());

        private static Task<PbGetMessageEvent> PullMessage(BusinessComponent context)
            => context.SendPacket<PbGetMessageEvent>(PbGetMessageEvent.Create(context.ConfigComponent.SyncCookie));

        #endregion
    }
}
